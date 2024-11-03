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
    private GameObject murBrisablePrefab;
    [SerializeField]
    private float murBrisableChance = 0.1f;
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
        // Création du parent container, de la grille et du chemin, de la salle, des toits, des murs et du joueur, en fonction de la taille de la grille, de la largeur et de la hauteur
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
        // Création de la salle, du chemin, des toits, des murs et du joueur
        CreationChemin();
        CreationToit();
        CreationMur();
        InstancierJoueur();
    }

    /// <summary>
    /// Permet de créer la grille avec largeur hauteur
    /// </summary>
    private void CreationGrid()
    {
        grid = new Cell[largeur, hauteur];
        // Création de la grille avec les sols
        for (int x = 0; x < largeur; x++)
        {
            for (int y = 0; y < hauteur; y++)
            {
                // Position du sol en fonction de la taille du sol
                Vector3 position = new Vector3(x * sizePrefabs, 0, y * sizePrefabs);
                GameObject solTile = Instantiate(solPrefab, position, Quaternion.identity, parentContainer.transform);

                // Nomme le sol en fonction de sa position et l'ajoute à la grille
                solTile.name = $"Sol (X: {x}, Y: {y}";
                grid[x, y] = new Cell(x, y, solTile);
            }
        }
    }

    /// <summary>
    /// Création du chemin
    /// </summary>
    private void CreationChemin()
    {
        //Détruit les sols qui ne font pas partie du chemin
        for (int x = 0;x < largeur; x++)
        {
            for(int y = 0;y < hauteur; y++)
            {
                // Si le sol n'est pas dans le chemin, on le détruit
                if (!path.Contains(grid[x, y]))
                {
                    Destroy(grid[x, y].sol);
                    grid[x, y] = null;
                }
                // Sinon on place un mur brisable aléatoirement
                else
                {
                    MurBrisableChance(x, y);
                }
            }
        }
    }

    /// <summary>
    /// Place les toits sur les sols
    /// </summary>
    private void CreationToit()
    {
        foreach(Cell cell in path)
        {
            // Si la cellule est null ou le sol est null, on continue
            if (cell == null || cell.sol == null) continue;

            // Position du toit en fonction de la position du sol
            Vector3 position = cell.sol.transform.position; // On reprend la position du sol et dans le prefab on met la bonne hauteur
            Instantiate(toitPrefab, position, Quaternion.identity, parentContainer.transform);
        }
    }

    /// <summary>
    /// Création des murs autour du chemin
    /// </summary>
    private void CreationMur()
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

                // Regarde si les voisins est INBOUND ou OUTBOUND
                if (nx < 0 || nx >= largeur || ny < 0 || ny >= hauteur || !path.Contains(grid[nx, ny]))
                {

                    bool placerTorche = torcheCount % torcheSpacing == 0; // Espace entre les torches avec modulo qui permet de savoir si on doit placer une torche en regardant si le count est divisible par l'espace entre les torches
                    torcheCount++;

                    // Position du mur en fonction de la direction et de la taille du sol
                    Vector3 position = cell.sol.transform.position + new Vector3(dx * sizePrefabs / 2, 0, dy * sizePrefabs / 2);

                    // Rotation du mur en fonction de la direction
                    Quaternion rotation = GetMurRotation(dx, dy);

                    // Instancie le mur ou la torche en fonction de la position et de la rotation
                    GameObject mur = placerTorche ? murTorchPrefab : murPrefab;

                    Instantiate(mur, position, rotation, parentContainer.transform);
                }
            }
        }
    }

    /// <summary>
    /// Instancie le joueur dans la première cellule
    /// </summary>
    private void InstancierJoueur()
    {
        // Si le prefab du joueur est pas null et la cellule de départ est pas null
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
    private Quaternion GetMurRotation(int dx, int dy)
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

        // Retourne la rotation du mur
        return Quaternion.Euler(0, angle, 0);
    }

    /// <summary>
    /// Trouve le chemin le plus court entre deux points
    /// https://www.geeksforgeeks.org/a-search-algorithm/
    /// Alogorithme A* pour trouver le chemin le plus court
    /// </summary>
    /// <param name="start">Cellule du début</param>
    /// <param name="end">Cellule fin</param>
    /// <returns>Retorune null ou RetracerChemain()</returns>
    private List<Cell> TrouverChemin(Cell start, Cell end)
    {
        // Liste pour les Cell a visiter
        List<Cell> visitSet = new List<Cell> { start };
        // Liste pour les Cell déjà visité
        HashSet<Cell> exploreSet = new HashSet<Cell>();

        while (visitSet.Count > 0)
        {
            Cell currentCell = visitSet[0];

            // Si la cellule actuelle est la fin, on retourne le chemin
            if (currentCell == end)
            {
                return RetracerChemin(start, end);
            }

            // Retire la première cellule de la liste openSet et l'ajoute à la liste closedSet
            visitSet.Remove(currentCell);
            exploreSet.Add(currentCell);

            // Pour chaque voisin de la cellule actuelle
            foreach (Cell voisin in GetRandomVoisins(currentCell))
            {
                if (!voisin.isWalkable || exploreSet.Contains(voisin))
                    continue;

                // Si le voisin est pas dans la liste visitSet, on l'ajoute a son parent
                if (!visitSet.Contains(voisin))
                {
                    voisin.parent = currentCell; 
                    visitSet.Add(voisin);
                }
            }
        }
        return null; // Aucun chemin trouver
    }

    /// <summary>
    /// Retrace le chemain de la fin à la début
    /// </summary>
    /// <param name="start">Cellule du début</param>
    /// <param name="end">Cellule de la fin</param>
    /// <returns>Retourne le chemain de point A au point B</returns>
    private List<Cell> RetracerChemin(Cell start, Cell end)
    {
        List<Cell> path = new List<Cell>();
        Cell currentNode = end;

        // Retrace le chemin de la fin à la début
        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent; 
        }

        // Ajoute le point de départ et inverse le chemin pour qu'il soit du début à la fin 
        path.Add(start);
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Randomize l'ordre des voisins pour rendre cela aléatoire
    /// </summary>
    /// <param name="node">Cellule de la grille</param>
    /// <returns>Retourne voisins</returns>
    private List<Cell> GetRandomVoisins(Cell node)
    {
        List<Cell> neighbors = new List<Cell>();

        // Ajoute les voisins de la cellule actuelle
        if (node.x > 0)
            neighbors.Add(grid[node.x - 1, node.y]);
        if (node.x < largeur - 1) 
            neighbors.Add(grid[node.x + 1, node.y]);
        if (node.y > 0) 
            neighbors.Add(grid[node.x, node.y - 1]);
        if (node.y < hauteur - 1) 
            neighbors.Add(grid[node.x, node.y + 1]);

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
    /// Place un mur brisable aléatoirement sur le chemin du Joueur 
    /// </summary>
    /// <param name="x">Grid X</param>
    /// <param name="y">Grid Y</param>
    private void MurBrisableChance(int x, int y)
    {
        if (Random.value < murBrisableChance)
        {
            // Direction du mur brisable
            int dx = 0, dy = 0;
            if (y < hauteur - 1 && path.Contains(grid[x, y + 1]))
                dy = 1;    
            else if (y > 0 && path.Contains(grid[x, y - 1]))
                dy = -1;       
            else if (x < largeur - 1 && path.Contains(grid[x + 1, y]))
                dx = 1; 
            else if (x > 0 && path.Contains(grid[x - 1, y])) 
                dx = -1;

            // Position du mur brisable en fonction de la direction du mur
            float murOffset = sizePrefabs / 2 - 0.1f; 
            Vector3 offset = new Vector3(dx * murOffset, 0f, dy * murOffset);
            Vector3 position = grid[x, y].sol.transform.position + offset;

            // Rotation du mur brisable et l'instancie avec la position et la rotation
            Quaternion rotation = GetMurRotation(dx, dy);
            Instantiate(murBrisablePrefab, position, rotation, parentContainer.transform);
        }
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
