using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoundsConfig : ConfigBase {
    public BoundsConfig(SimpleJson.JsonObject obj)
    {
    }
}

public class PlaceHolderConfig : ConfigBase {
    public PlaceHolderConfig(SimpleJson.JsonObject obj)
    {
    }
}

public class SurfaceCellPlane : ConfigBase {
    public SurfaceCellPlane(SimpleJson.JsonObject obj)
    {
        object v = null;
        SimpleJson.JsonArray triggerlist;
        if (obj.TryGetValue("Materials", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Materials = new List<string>();
            foreach (object item in triggerlist)
            {
                Materials.Add(Convert.ToString(item));
            }
        }
        else Debug.LogError("!");

        if (obj.TryGetValue("CellWidth", out v)) this.CellWidth = Convert.ToDouble(v); else Debug.LogError("!");
        if (obj.TryGetValue("CellHeight", out v)) this.CellHeight = Convert.ToDouble(v); else Debug.LogError("!");
    }

    public List<string> Materials { get; set; }
    public double CellWidth { get; set; }
    public double CellHeight { get; set; }
}

public class SurfaceCellPlaneWithCollider : ConfigBase {

    public SurfaceCellPlaneWithCollider(SimpleJson.JsonObject obj)
    {
        object v = null;
        if (obj.TryGetValue("Plane", out v)) this.Plane = new SurfaceCellPlane((SimpleJson.JsonObject)v); else Debug.LogError("!");

        SimpleJson.JsonArray triggerlist;
        if (obj.TryGetValue("Colliders", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Colliders = new List<SurfaceCellCollider>();
            foreach (object item in triggerlist)
            {
                Colliders.Add(new SurfaceCellCollider((SimpleJson.JsonObject)item));
            }
        }
        else Debug.LogError("!");
    }

    public SurfaceCellPlane Plane { get; set; }
    public List<SurfaceCellCollider> Colliders { get; set; }
}

public class SurfaceCellCollider : ConfigBase {

    public SurfaceCellCollider(SimpleJson.JsonObject obj)
    {
        object v = null;
        SimpleJson.JsonArray triggerlist;
        if (obj.TryGetValue("Center", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Center = new List<double>();
            foreach (object item in triggerlist)
            {
                Center.Add(Convert.ToDouble(item));
            }
        }
        else Debug.LogError("!");

        if (obj.TryGetValue("Size", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Size = new List<double>();
            foreach (object item in triggerlist)
            {
                Size.Add(Convert.ToDouble(item));
            }
        }
        else Debug.LogError("!");

        if (obj.TryGetValue("IsTrigger", out v)) this.IsTrigger = Convert.ToBoolean(v); else Debug.LogError("!");
    }

    public List<double> Center { get; set; }
    public List<double> Size { get; set; }
    public bool IsTrigger { get; set; }
}

public class SurfaceConf : ConfigBase {
    public SurfaceConf(SimpleJson.JsonObject obj)
    {
        object v = null;
        SimpleJson.JsonArray triggerlist;
        if (obj.TryGetValue("Colliders", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Colliders = new List<SurfaceCellCollider>();
            foreach (object item in triggerlist)
            {
                Colliders.Add(new SurfaceCellCollider((SimpleJson.JsonObject)item));
            }
        }
        else Debug.LogError("!");

        if (obj.TryGetValue("PlaneWithColliders", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            PlaneWithColliders = new List<SurfaceCellPlaneWithCollider>();
            foreach (object item in triggerlist)
            {
                PlaneWithColliders.Add(new SurfaceCellPlaneWithCollider((SimpleJson.JsonObject)item));
            }
        }
        else Debug.LogError("!");

        if (obj.TryGetValue("Planes", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Planes = new List<SurfaceCellPlane>();
            foreach (object item in triggerlist)
            {
                Planes.Add(new SurfaceCellPlane((SimpleJson.JsonObject)item));
            }
        }
        else Debug.LogError("!");
    }

    public List<SurfaceCellCollider> Colliders { get; set; }
    public List<SurfaceCellPlaneWithCollider> PlaneWithColliders { get; set; }
    public List<SurfaceCellPlane> Planes { get; set; }
}


public class GroundCell : ConfigBase {
    public GroundCell(SimpleJson.JsonObject obj)
    {
        object v = null;
        SimpleJson.JsonArray triggerlist;
        if (obj.TryGetValue("Materials", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Materials = new List<string>();
            foreach (object item in triggerlist)
            {
                Materials.Add(Convert.ToString(item));
            }
        }
        else Debug.LogError("!");
    }

    public List<string> Materials { get; set; }
}

public class GroundConf : ConfigBase {
    public GroundConf(SimpleJson.JsonObject obj)
    {
        object v = null;
        if (obj.TryGetValue("CellWidth", out v)) this.CellWidth = Convert.ToDouble(v); else Debug.LogError("!");
        if (obj.TryGetValue("CellHeight", out v)) this.CellHeight = Convert.ToDouble(v); else Debug.LogError("!");

        SimpleJson.JsonArray triggerlist;
        if (obj.TryGetValue("GroundCells", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            GroundCells = new List<GroundCell>();
            foreach (object item in triggerlist)
            {
                GroundCells.Add(new GroundCell((SimpleJson.JsonObject)item));
            }
        }
        else Debug.LogError("!");
    }
    public double CellWidth { get; set; }
    public double CellHeight { get; set; }
    public List<GroundCell> GroundCells { get; set; }
}

public class MapConf : ConfigBase {
    public MapConf(SimpleJson.JsonObject obj)
    {
        object v = null;
        if (obj.TryGetValue("Ground", out v)) this.Ground = new GroundConf((SimpleJson.JsonObject)v); else Debug.LogError("!");
        if (obj.TryGetValue("Surface", out v)) this.Surface = new SurfaceConf((SimpleJson.JsonObject)v); else Debug.LogError("!");
    }
    public GroundConf Ground { get; set; }
    public SurfaceConf Surface { get; set; }
}

public class PrefabConf : ConfigBase {
    public PrefabConf(SimpleJson.JsonObject obj)
    {
        object v = null;
        if (obj.TryGetValue("PrefabName", out v)) this.PrefabName = Convert.ToString(v); else Debug.LogError("!");
    }
    public string PrefabName { get; set; }
}

public class SceneConf : ConfigBase {
    public SceneConf(SimpleJson.JsonObject obj)
    {
        object v = null;
        if (obj.TryGetValue("Map", out v)) this.Map = new MapConf((SimpleJson.JsonObject)v); else Debug.LogError("!");
        if (obj.TryGetValue("UI", out v)) this.UI = new PrefabConf((SimpleJson.JsonObject)v); else Debug.LogError("!");
        if (obj.TryGetValue("Light", out v)) this.Light = new PrefabConf((SimpleJson.JsonObject)v); else Debug.LogError("!");

        SimpleJson.JsonArray triggerlist;
        if (obj.TryGetValue("Bounds", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            Bounds = new BoundsConfig[triggerlist.Count];
            int index = 0;
            foreach (object item in triggerlist)
            {
                Bounds[index++] = new BoundsConfig((SimpleJson.JsonObject)item);
            }
        }
        else Debug.LogError("!");

        if (obj.TryGetValue("PlaceHolder", out v))
        {
            triggerlist = (SimpleJson.JsonArray)v;
            PlaceHolder = new PlaceHolderConfig[triggerlist.Count];
            int index = 0;
            foreach (object item in triggerlist)
            {
                PlaceHolder[index++] = new PlaceHolderConfig((SimpleJson.JsonObject)item);
            }
        }
        else Debug.LogError("!");
    }

    public MapConf Map { get; set; }
    public PrefabConf UI { get; set; }
    public PrefabConf Light { get; set; }
    public BoundsConfig[] Bounds { get; set; }
    public PlaceHolderConfig[] PlaceHolder { get; set; }
}