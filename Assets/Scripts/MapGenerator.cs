using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
    public Tilemap tilemap;
    public TileBase defaultTile;
    public TileBase startTile;
    public GameObject player;

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    public int smoothIterations;

    [Range(0, 8)]
    public int smoothDeathThreshold;
    [Range(0, 8)]
    public int smoothBirthThreshold;

    private System.Random randgen;

    int[,] map;

    void Start() {
        GenerateMap();
    }

    private void Update() {
        if (Input.GetButtonDown("Fire1")) {
            GenerateMap();
        }
    }

    void GenerateMap() {
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }
        randgen = new System.Random(seed.GetHashCode());

        tilemap.ClearAllTiles();
        map = new int[width, height];
        RandomFillMap();

        int posx = randgen.Next(width - 2) + 1;
        int posy = randgen.Next(height - 3) + 1;
        map[posx, posy - 1] = 2;
        map[posx, posy] = 3;
        map[posx, posy + 1] = 4;

        //StartCoroutine(CallSmooth());
        for (int i = 0; i < smoothIterations; i++) {
            SmoothMap();
        }
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (map[x, y] == 1) {
                    int dx = x * 2;
                    int dy = y * 2;
                    tilemap.SetTile(new Vector3Int(dx - 1, dy - 1, 0), defaultTile);
                    tilemap.SetTile(new Vector3Int(dx - 1, dy, 0), defaultTile);
                    tilemap.SetTile(new Vector3Int(dx, dy - 1, 0), defaultTile);
                    tilemap.SetTile(new Vector3Int(dx, dy, 0), defaultTile);
                } else if (map[x, y] == 2) {
                    int dx = x * 2;
                    int dy = y * 2;
                    tilemap.SetTile(new Vector3Int(dx - 1, dy - 1, 0), startTile);
                    tilemap.SetTile(new Vector3Int(dx - 1, dy, 0), startTile);
                    tilemap.SetTile(new Vector3Int(dx, dy - 1, 0), startTile);
                    tilemap.SetTile(new Vector3Int(dx, dy, 0), startTile);
                } else if (map[x, y] == 3) {
                    int dx = x * 2;
                    int dy = y * 2;
                    //tilemap.SetTile(new Vector3Int(dx - 1, dy - 1, 0), defaultTile);
                    //tilemap.SetTile(new Vector3Int(dx - 1, dy, 0), defaultTile);
                    //tilemap.SetTile(new Vector3Int(dx, dy - 1, 0), defaultTile);
                    //tilemap.SetTile(new Vector3Int(dx, dy, 0), defaultTile);
                    Vector3 cellPosition = tilemap.CellToWorld(new Vector3Int(dx, dy, 0));
                    player.transform.position = cellPosition;
                }
            }
        }

        CompositeCollider2D collider = tilemap.GetComponent<CompositeCollider2D>();
        collider.GenerateGeometry();
    }

    IEnumerator CallSmooth() {
        for (int i = 0; i < smoothIterations; i++) {
            SmoothMap();
            yield return new WaitForSeconds(.5f);
        }
        //yield return null;
    }

    void RandomFillMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(IsBorder(x, y)) {
                    map[x, y] = 1;
                } else { 
                    map[x, y] = (randgen.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurrondingWallCount(x, y);
                if (map[x, y] == 2 || map[x, y] == 3) { continue; }
                if (neighbourWallTiles > smoothBirthThreshold) {
                    map[x, y] = 1;
                } else if (neighbourWallTiles < smoothDeathThreshold) {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurrondingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int nX = gridX - 1; nX <= gridX + 1; nX++) {
            for (int nY = gridY - 1; nY <= gridY + 1; nY++) {
                if (!IsInMap(nX, nY) && (nX != gridX || nY != gridY)) {
                    wallCount += 1;
                } else if (map[nX, nY] == 1 || map[nX, nY] == 2) { 
                    wallCount += 1;
                }
            }
        }
        return wallCount;
    }

    bool IsBorder(int x, int y) {
        return (x == 0 || y == 0 || x == width - 1 || y == height - 1);
    }

    bool IsInMap(int x, int y) {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }
}