using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstruirEntorno : MonoBehaviour
{
    private void Awake()
    {
        GameObject cuadricula = new GameObject("CuadriculaNivel", typeof(Grid));
        GameObject suelo = new GameObject("SueloTilemap", typeof(Tilemap), typeof(TilemapRenderer));
        suelo.transform.SetParent(cuadricula.transform, false);
        Tilemap mapa = suelo.GetComponent<Tilemap>();
        Sprite[] piezas = Resources.LoadAll<Sprite>("Terreno");
        if (piezas.Length == 0) { Debug.LogError("No se encontraron las piezas del suelo."); return; }
        Tile[] tiles = new Tile[piezas.Length];
        for (int i = 0; i < piezas.Length; i++)
        {
            tiles[i] = ScriptableObject.CreateInstance<Tile>();
            tiles[i].sprite = piezas[i];
            tiles[i].colliderType = Tile.ColliderType.Grid;
        }
        for (int x = -14; x <= 100; x++)
        {
            mapa.SetTile(new Vector3Int(x, -3, 0), tiles[0]);
            for (int y = -4; y >= -12; y--)
                mapa.SetTile(new Vector3Int(x, y, 0), tiles[Mathf.Min(1, tiles.Length-1)]);
        }
        TilemapCollider2D contacto = suelo.AddComponent<TilemapCollider2D>();
        Rigidbody2D fisica = suelo.AddComponent<Rigidbody2D>();
        fisica.bodyType = RigidbodyType2D.Static;
        CompositeCollider2D combinado = suelo.AddComponent<CompositeCollider2D>();
        combinado.geometryType = CompositeCollider2D.GeometryType.Polygons;
        contacto.compositeOperation = Collider2D.CompositeOperation.Merge;
    }
}
