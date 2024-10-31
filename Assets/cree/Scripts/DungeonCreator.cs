using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private GameObject playerPrefab;

    [Header("Taille de la grille")]
    [SerializeField]
    private int largeur = 10;
    [SerializeField]
    private int hauteur = 10;

    [Header("Prefabs & Grandeur")]
    [SerializeField]
    private float sizePrefabs = 10f; // Par défault un plane == 10 x 10 Unity Scale
    [SerializeField]
    private GameObject solPrefab;
    [SerializeField]
    private GameObject murPrefab;
    [SerializeField]
    private GameObject murTorchPrefab;
    [SerializeField]
    private GameObject toitPrefab;

    private Cell[,] grid;
    private Cell startCell;
    private Cell endCell;
    private List<Cell> path;
    private GameObject parentContainer;

    /// <summary>
    /// Fonciton Awake
    /// </summary>
    private void Awake()
    {
        parentContainer = new GameObject("Salle Dungeon");

        CreationGrid();
        startCell = grid[0, 0];
        endCell = grid[largeur - 1, hauteur - 1];
        path = TrouverChemin(startCell, endCell);
    }

    /// <summary>
    /// Fonction Start
    /// </summary>
    private void Start()
    {
        CreationChemin();
        CreationToit();
        CreationMur();
        InstancierJoueur();
    }

    /// <summary>
    /// Permet de créer la grille avec largeur hauteur
    /// </summary>
    void CreationGrid()
    {
        grid = new Cell[largeur, hauteur];
        for (int x = 0; x < largeur; x++)
        {
            for (int y = 0; y < hauteur; y++)
            {
                Vector3 position = new Vector3(x * sizePrefabs, 0, y * sizePrefabs);
                GameObject solTile = Instantiate(solPrefab, position, Quaternion.identity, parentContainer.transform);

                solTile.name = $"Sol (X: {x}, Y: {y}";

                grid[x, y] = new Cell(x, y, solTile);
            }
        }
    }

    /// <summary>
    /// Création du chemin
    /// </summary>
    void CreationChemin()
    {
        //Détruit les sols qui ne font pas partie du chemin
        for (int x = 0;x < largeur; x++)
        {
            for(int y = 0;y < hauteur; y++)
            {
                if (!path.Contains(grid[x, y]))
                {
                    Destroy(grid[x, y].sol);
                    grid[x, y] = null;
                }
            }
        }
    }

    /// <summary>
    /// Place les toits sur les sols
    /// </summary>
    void CreationToit()
    {
        foreach(Cell cell in path)
        {
            if (cell == null || cell.sol == null) continue;

            Vector3 position = cell.sol.transform.position; // On reprend la position du sol et dans le prefab on met la bonne hauteur
            Instantiate(toitPrefab, position, Quaternion.identity, parentContainer.transform);
        }
    }

    /// <summary>
    /// Création des murs autour du chemin
    /// </summary>
    void CreationMur()
    {
        int torcheCount = 0;
        int torcheSpacing = 3; /* Espace entre les torches */

        foreach (Cell cell in path)
        {
            if (cell == null || cell.sol == null) continue;

            int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
            for (int i = 0; i < 4; i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];
                int nx = cell.x + dx;
                int ny = cell.y + dy;

                // Regarde si voisins est INBOUND ou OUTBOUND
                if (nx < 0 || nx >= largeur || ny < 0 || ny >= hauteur || !path.Contains(grid[nx, ny]))
                {

                    bool placerTorche = false;
                    if (torcheCount % torcheSpacing == 0)
                    {
                        placerTorche = true;
                    }
                    torcheCount++;

                    Vector3 position = cell.sol.transform.position + new Vector3(dx * sizePrefabs / 2, 0, dy * sizePrefabs / 2);

                    Quaternion rotation = GetMurRotation(dx, dy);

                    GameObject mur = placerTorche ? murTorchPrefab : murPrefab;

                    Instantiate(mur, position, rotation, parentContainer.transform);
                }
            }
        }
    }

    /// <summary>
    /// Instancie le joueur dans la première cellule
    /// </summary>
    void InstancierJoueur()
    {
        if (playerPrefab != null && startCell != null)
        {
            Vector3 playerPosition = startCell.sol.transform.position + new Vector3(0, 1f, 0); //Éviter qu'il soit dans le sol
            Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// Retourne la rotation du mur en fonction de la direction
    /// </summary>
    /// <param name="dx">Direction X</param>
    /// <param name="dy">Direction Y</param>
    /// <returns></returns>
    Quaternion GetMurRotation(int dx, int dy)
    {
        float angle = 0f;

        if (dx == 1 && dy == 0)      // Est
            angle = -90f;
        else if (dx == -1 && dy == 0) // Ouest
            angle = 90f;
        else if (dx == 0 && dy == 1)  // Nord
            angle = 180f;
        else if (dx == 0 && dy == -1) // Sud
            angle = 0f;

        return Quaternion.Euler(0, angle, 0);
    }

    /// <summary>
    /// Trouve le chemin le plus court entre deux points
    /// </summary>
    /// <param name="start">Cellule du début</param>
    /// <param name="end">Cellule fin</param>
    /// <returns>Retorune null ou RetracerChemain()</returns>
    List<Cell> TrouverChemin(Cell start, Cell end)
    {
        List<Cell> openSet = new List<Cell> { start };
        HashSet<Cell> closedSet = new HashSet<Cell>();

        while (openSet.Count > 0)
        {
            Cell currentCell = openSet[0];

            if (currentCell == end)
            {
                return RetracerChemin(start, end);
            }

            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            foreach (Cell voisin in GetRandomizedNeighbors(currentCell))
            {
                if (!voisin.isWalkable || closedSet.Contains(voisin))
                    continue;

                if (!openSet.Contains(voisin))
                {
                    voisin.parent = currentCell;
                    openSet.Add(voisin);
                }
            }
        }
        return null; // Aucun chemin toruver
    }

    /// <summary>
    /// Retrace le chemain de la fin à la début
    /// </summary>
    /// <param name="start">Cellule du début</param>
    /// <param name="end">Cellule de la fin</param>
    /// <returns>Retourne le chemain de point A au point B</returns>
    List<Cell> RetracerChemin(Cell start, Cell end)
    {
        List<Cell> path = new List<Cell>();
        Cell currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent; 
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Randomize l'ordre des voisins pour rendre cela aléatoire
    /// </summary>
    /// <param name="node">Cellule de la grille</param>
    /// <returns>Retourne voisins</returns>
    List<Cell> GetRandomizedNeighbors(Cell node)
    {
        List<Cell> neighbors = new List<Cell>();

        if (node.x > 0) neighbors.Add(grid[node.x - 1, node.y]);
        if (node.x < largeur - 1) neighbors.Add(grid[node.x + 1, node.y]);
        if (node.y > 0) neighbors.Add(grid[node.x, node.y - 1]);
        if (node.y < hauteur - 1) neighbors.Add(grid[node.x, node.y + 1]);

        // Randomize l'ordre des voisins pour rendre cela aléatoire
        for (int i = 0; i < neighbors.Count; i++)
        {
            Cell temp = neighbors[i];
            int randomIndex = Random.Range(0, neighbors.Count);
            neighbors[i] = neighbors[randomIndex];
            neighbors[randomIndex] = temp;
        }

        return neighbors;
    }


    /// <summary>
    /// Représente une cellule dans la grille
    /// </summary>
    public class Cell
    {
        public int x, y;
        public bool isWalkable = true;
        public Cell parent;
        public GameObject sol;

        public Cell(int x, int y, GameObject sol)
        {
            this.x = x;
            this.y = y;
            this.sol = sol;
        }
    }
}
