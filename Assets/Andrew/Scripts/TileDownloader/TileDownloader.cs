using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TileDownloader: MonoBehaviour
{
    #region Variables
    private string folder;

    private double leftLongitude;
    private double topLatitude;
    private double rightLongitude;
    private double bottomLatitude;
    private int minZoom = 3;
    private int maxZoom = 20;

    private const int averageSize = 17; // KB //30000;
    public int countTiles = 0;
    public int downloadedTiles = 0;
    public int deletedTiles = 0;
    public long totalSize = 0;
    private static OnlineMaps map;
    private OnlineMapsDrawingRect rect;

    private List<Tile> downloadTiles;
    private List<Tile> deleteTiles;

    public bool isDownloading;
    public bool isDeleting;
    #endregion

    #region Unity Functions
    private void Start()
    {
        map = OnlineMaps.instance;
        isDownloading = false;

        folder = Application.persistentDataPath + "/Tiles/"; // Application.streamingAssetsPath + "/OnlineMapsTiles/"; //Application.persistentDataPath + "/Tiles/";

        if (OnlineMapsCache.instance != null) OnlineMapsCache.instance.OnStartDownloadTile += OnStartDownloadTile;
        else OnlineMapsTileManager.OnStartDownloadTile += OnStartDownloadTile;
    }

    /*private void Update()
    {
        // Get view size
        leftLongitude = map.bounds.left;
        topLatitude = map.bounds.top;
        rightLongitude = map.bounds.right;
        bottomLatitude = map.bounds.bottom;
        minZoom = map.zoom;
        maxZoom = 20;
        //map.resourcesPath.Replace(map.resourcesPath, );
    }*/

    /*private void OnGUI()
    {
        //GUILayout.BeginArea(new Rect(10, 10, 300, Screen.height - 20));
        GUILayout.BeginArea(new Rect(10, 10, 1080, 1920));

        *//*double newLeftLongitude = DoubleField("Left Longitude", leftLongitude);
        double newTopLatitude = DoubleField("Top Latitude", topLatitude);
        double newRightLongitude = DoubleField("Right Longitude", rightLongitude);
        double newBottomLatitude = DoubleField("Bottom Latitude", bottomLatitude);

        bool updateRect = false;

        if (Math.Abs(newLeftLongitude - leftLongitude) > double.Epsilon)
        {
            leftLongitude = newLeftLongitude;
            updateRect = true;
        }

        if (Math.Abs(newRightLongitude - rightLongitude) > double.Epsilon)
        {
            rightLongitude = newRightLongitude;
            updateRect = true;
        }

        if (Math.Abs(newBottomLatitude - bottomLatitude) > double.Epsilon)
        {
            bottomLatitude = newBottomLatitude;
            updateRect = true;
        }

        if (Math.Abs(newTopLatitude - topLatitude) > double.Epsilon)
        {
            topLatitude = newTopLatitude;
            updateRect = true;
        }*//*

        //if (updateRect) UpdateRect();

        //minZoom = IntField("Min Zoom", minZoom);
        //maxZoom = IntField("Max Zoom", maxZoom);

        if (GUILayout.Button("Place")) PlaceRect();
        if (GUILayout.Button("Calculate")) Calculate();

        GUILayout.Label("Count Tiles: " + countTiles);
        GUILayout.Label("Total Size: " + totalSize);

        if (GUILayout.Button("Download")) Download();

        GUILayout.EndArea();
    }*/
    #endregion

    #region Methods

    public void SetValues(double _leftLongitude, double _topLatitude, double _rightLongitude, double _bottomLatitude, int _minZoom, int _maxZoom)
    {
        leftLongitude = _leftLongitude; //map.bounds.left;
        topLatitude = _topLatitude; // map.bounds.top;
        rightLongitude = _rightLongitude; // map.bounds.right;
        bottomLatitude = _bottomLatitude; // map.bounds.bottom;
        minZoom = _minZoom; // map.zoom;
        maxZoom = _maxZoom; // 20;
    }

    public void Calculate()
    {
        countTiles = 0;
        for (int z = minZoom; z <= maxZoom; z++)
        {
            double tlx, tly, brx, bry;
            map.projection.CoordinatesToTile(leftLongitude, topLatitude, z, out tlx, out tly);
            map.projection.CoordinatesToTile(rightLongitude, bottomLatitude, z, out brx, out bry);

            int itlx = (int) tlx;
            int itly = (int) tly;
            int ibrx = (int)Math.Ceiling(brx);
            int ibry = (int)Math.Ceiling(bry);

            countTiles += (ibrx - itlx) * (ibry - itly);
        }

        totalSize = countTiles * averageSize;

        //Debug.Log("countTiles = " + countTiles);
        //Debug.Log("totalSize = " + totalSize);
        countTiles = 0;
    }

    /*private double DoubleField(string label, double value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(label, GUILayout.Width(150));
        string strVal = GUILayout.TextField(value.ToString());

        GUILayout.EndHorizontal();

        double newValue;
        if (double.TryParse(strVal, out newValue)) return newValue;
        return value;
    }*/

    public void Download()
    {
        downloadTiles = new List<Tile>();
        downloadedTiles = 0;
        countTiles = 0;

        for (int z = minZoom; z <= maxZoom; z++)
        {
            double tlx, tly, brx, bry;
            map.projection.CoordinatesToTile(leftLongitude, topLatitude, z, out tlx, out tly);
            map.projection.CoordinatesToTile(rightLongitude, bottomLatitude, z, out brx, out bry);

            int itlx = (int)tlx;
            int itly = (int)tly;
            int ibrx = (int)Math.Ceiling(brx);
            int ibry = (int)Math.Ceiling(bry);

            for (int x = itlx; x < ibrx; x++)
            {
                for (int y = itly; y < ibry; y++)
                {
                    downloadTiles.Add(new Tile
                    {
                        x = x,
                        y = y,
                        zoom = z
                    });
                }
            }

            countTiles += (ibrx - itlx) * (ibry - itly);
        }

        if (downloadTiles.Count > 0)
            isDownloading = true;

        StartNextDownload();
    }

    public void DeleteTiles(cArea _areaToDelete)
    {
        SetValues(_areaToDelete.areaConstraintsMin.x, _areaToDelete.areaConstraintsMax.y, _areaToDelete.areaConstraintsMax.x, _areaToDelete.areaConstraintsMin.y, OnlineMaps.MAXZOOM, OnlineMaps.MAXZOOM);
        //Calculate();
        StartCoroutine(Delete());
    }

    IEnumerator Delete()
    {
        deleteTiles = new List<Tile>();
        deletedTiles = 0;
        countTiles = 0;

        for (int z = minZoom; z <= maxZoom; z++)
        {
            double tlx, tly, brx, bry;
            map.projection.CoordinatesToTile(leftLongitude, topLatitude, z, out tlx, out tly);
            map.projection.CoordinatesToTile(rightLongitude, bottomLatitude, z, out brx, out bry);

            int itlx = (int)tlx;
            int itly = (int)tly;
            int ibrx = (int)Math.Ceiling(brx);
            int ibry = (int)Math.Ceiling(bry);

            for (int x = itlx; x < ibrx; x++)
            {
                for (int y = itly; y < ibry; y++)
                {
                    deleteTiles.Add(new Tile
                    {
                        x = x,
                        y = y,
                        zoom = z
                    });
                }
            }

            countTiles += (ibrx - itlx) * (ibry - itly);
        }

        if (deleteTiles.Count > 0)
        {
            isDeleting = true;

            // Calculate seconds to upload
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            // Activate panel
            AppManager.Instance.uIManager.txtWarningServer.text = "";
            AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(true);

            while (deleteTiles.Count > 0)
            {
                deletedTiles += 1;
                
                // Update panel
                int percentage = Mathf.RoundToInt((float)(((double)deletedTiles / (double)countTiles) * 100));
                AppManager.Instance.uIManager.txtWarningServer.text = "Deleting tiles... \n" + percentage + "%";

                Tile tile = deleteTiles[0];
                deleteTiles.RemoveAt(0);
                string tilePath = GetTilePath(tile);
                if (File.Exists(tilePath))
                    File.Delete(tilePath);

                yield return null;
            }

            isDeleting = false;

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            yield return new WaitForSeconds(ts.TotalSeconds > 1f ? 0f : (1f - (float)ts.TotalSeconds));
            AppManager.Instance.uIManager.pnlWarningServerScreen.SetActive(false);
        }
    }

    private string GetTilePath(Tile tile)
    {
        return GetTilePath(tile.zoom, tile.x, tile.y);
    }

    private string GetTilePath(int zoom, int x, int y)
    {
        StringBuilder builder = new StringBuilder(folder);
        builder.Append(zoom).Append("/");
        builder.Append(x).Append("/");
        builder.Append(y).Append(".png");

        return builder.ToString();
    }

    /*private int IntField(string label, int value)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(label, GUILayout.Width(150));
        string strVal = GUILayout.TextField(value.ToString());

        GUILayout.EndHorizontal();

        int newValue;
        if (int.TryParse(strVal, out newValue)) return newValue;
        return value;
    }*/

    private void OnStartDownloadTile(OnlineMapsTile tile)
    {
        string tilePath = GetTilePath(tile.zoom, tile.x, tile.y);
        if (!File.Exists(tilePath))
        {
            OnlineMapsTileManager.StartDownloadTile(tile);
            return;
        }

        byte[] bytes = File.ReadAllBytes(tilePath);

        Texture2D tileTexture = new Texture2D(256, 256);
        tileTexture.LoadImage(bytes);
        tileTexture.wrapMode = TextureWrapMode.Clamp;

        if (map.control.resultIsTexture)
        {
            (tile as OnlineMapsRasterTile).ApplyTexture(tileTexture);
            map.buffer.ApplyTile(tile);
            OnlineMapsUtils.Destroy(tileTexture);
        }
        else
        {
            tile.texture = tileTexture;
            tile.status = OnlineMapsTileStatus.loaded;
        }

        tile.MarkLoaded();

        map.Redraw();
    }

    private void OnTileDownloaded(OnlineMapsWWW www)
    {
        string tilePath = www["path"] as string;
        if (!www.hasError)
        {
            FileInfo fileInfo = new FileInfo(tilePath);
            DirectoryInfo directoryInfo = fileInfo.Directory;
            if (!directoryInfo.Exists) directoryInfo.Create();

            File.WriteAllBytes(tilePath, www.bytes);
        }

        StartNextDownload();
    }

    /*private void PlaceRect()
    {
        map.GetTileCorners(out leftLongitude, out topLatitude, out rightLongitude, out bottomLatitude, 20);
        double rx = rightLongitude - leftLongitude;
        double ry = bottomLatitude - topLatitude;
        double cx = (rightLongitude + leftLongitude) / 2;
        double cy = (bottomLatitude + topLatitude) / 2;
        rx *= 0.8;
        ry *= 0.8;

        leftLongitude = cx - rx / 2;
        rightLongitude = cx + rx / 2;
        topLatitude = cy - ry / 2;
        bottomLatitude = cy + ry / 2;

        map.projection.TileToCoordinates(leftLongitude, topLatitude, 20, out leftLongitude, out topLatitude);
        map.projection.TileToCoordinates(rightLongitude, bottomLatitude, 20, out rightLongitude, out bottomLatitude);

        UpdateRect();

        map.Redraw();
    }*/

    private void StartNextDownload()
    {
        if (downloadTiles == null)
        {
            isDownloading = false;
            return;
        }

        while (downloadTiles.Count > 0)
        {
            downloadedTiles += 1;

            Tile tile = downloadTiles[0];
            downloadTiles.RemoveAt(0);
            string tilePath = GetTilePath(tile);
            if (File.Exists(tilePath)) continue;

            string url = tile.url;
            //Debug.Log(url + "    " + tilePath);
            OnlineMapsWWW www = new OnlineMapsWWW(url);
            www["path"] = tilePath;
            www.OnComplete += OnTileDownloaded;
            return;
        }

        isDownloading = false;
    }

    private void StartNextDelete()
    {
        if (deleteTiles == null)
        {
            isDeleting = false;
            return;
        }

        while (deleteTiles.Count > 0)
        {
            deletedTiles += 1;

            Tile tile = deleteTiles[0];
            deleteTiles.RemoveAt(0);
            string tilePath = GetTilePath(tile);
            if (File.Exists(tilePath))
                File.Delete(tilePath);
            return;
        }

        isDeleting = false;
    }

    /*private void UpdateRect()
    {
        if (rect == null)
        {
            rect = new OnlineMapsDrawingRect((float) leftLongitude, (float) bottomLatitude, (float) (rightLongitude - leftLongitude), (float) (topLatitude - bottomLatitude), Color.blue, 5, new Color(1, 1, 1, 0.1f));
            OnlineMapsDrawingElementManager.AddItem(rect);
        }
        else
        {
            rect.x = leftLongitude;
            rect.y = bottomLatitude;
            rect.width = rightLongitude - leftLongitude;
            rect.height = topLatitude - bottomLatitude;
        }
    }*/

    private struct Tile
    {
        public int x, y, zoom;

        public string url
        {
            get
            {
                OnlineMapsRasterTile tile = new OnlineMapsRasterTile(x, y, zoom, map, false);
                string tileURL = map.activeType.GetURL(tile);
                tile.Dispose();
                return tileURL;
            }
        }
    }
    #endregion
}