using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class SceneConfigManger : MonoBehaviour
{
    private List<Vector3> _cur_scene_bounds = new List<Vector3>();

    /// <summary>
    /// 当前场景地图边界
    /// </summary>
    public List<Vector3> MapBounds
    {
        get { return _cur_scene_bounds; }
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="fileName">相对于 Application.streamingAssetsPath 的路径  (/Scene/Scene_1_1.json)</param>
    ///// <returns></returns>
    //public string LoadJson(string fileName) {
    //    string path = Application.streamingAssetsPath + fileName;
    //    if (!File.Exists(path)) {
    //        Debug.LogError("文件不存在：" + path);
    //    }

    //    //加载配置文件
    //    return File.ReadAllText(path);
    //}

    /// <summary>
    /// 加载场景 
    /// </summary>
    /// <param name="sceneName">例如： Scene_1_1 酱紫 </param>
    public GameObject LoadScene(string sceneName, bool initUI)
    {
        Debug.Log("Load Scene  " + sceneName);
        // 先从 Resources / Prefabs / sceneName 读取场景预置 如果没有再从json中取（暂时 以后去掉）
        Object sceneObj = Resources.Load("Map/Prefabs/" + sceneName);
        if(sceneObj != null)
        {
            GameObject scene_go = GameObject.Find("Scene");

            if (scene_go == null)
            {
                Debug.LogError("导入场景数据出错，场景中无Scene节点！");
                return null;
            }

            GameObject sceneProfeb = Instantiate(sceneObj) as GameObject;
            if (sceneProfeb != null)
            {
                sceneProfeb.name = "SceneMesh";
                sceneProfeb.transform.parent = scene_go.transform;
                sceneProfeb.transform.localPosition = Vector3.zero;

                Transform trColliders = sceneProfeb.transform.FindChild("Colliders");
                if (trColliders != null)
                {
                    for (int n = 0; n < trColliders.childCount; n++ )
                    {
                        Transform tr = trColliders.GetChild(n);
                        tr.gameObject.AddComponent<FootprintComponent>();
                    }
                }

                Transform trBounds = sceneProfeb.transform.FindChild("Bounds");
                if (trBounds != null)
                {
                    for (int n = 0; n < trBounds.childCount; n++)
                    {
                        Transform tr = trBounds.GetChild(n);
                        _cur_scene_bounds.Add(tr.position);
                    }
                }

                Transform trPlaceHolder = sceneProfeb.transform.FindChild("PlaceHolder");
                if (trPlaceHolder != null)
                {
                    for (int n = 0; n < trPlaceHolder.childCount; n++)
                    {
                        Transform tr = trPlaceHolder.GetChild(n);
                        if (tr.gameObject.name == "100")
                            FightScene.s_InstanceThis.PushPos100(tr.position);
                        else if (tr.gameObject.name == "101")
                            FightScene.s_InstanceThis.PushPos101(tr.position);
                        else if (tr.gameObject.name == "102")
                            FightScene.s_InstanceThis.PushPos102(tr.position);
                        else
                            FightScene.s_InstanceThis.PushPos(tr.position);
                    }
                }

                // SceneObjects/Events/Events_01 这些点记录下 在表中会指定代表会加哪种buff 01-06
                // Condition/00 记录点 代表要走到的目标点位置 半径10 
                Transform trEvent1 = sceneProfeb.transform.FindChild("SceneObjects/Events/Events_01");
                if (trEvent1 != null) FightScene.s_InstanceThis.PushEventPos(trEvent1.position);
                Transform trEvent2 = sceneProfeb.transform.FindChild("SceneObjects/Events/Events_02");
                if (trEvent2 != null) FightScene.s_InstanceThis.PushEventPos(trEvent2.position);
                Transform trEvent3 = sceneProfeb.transform.FindChild("SceneObjects/Events/Events_03");
                if (trEvent3 != null) FightScene.s_InstanceThis.PushEventPos(trEvent3.position);
                Transform trEvent4 = sceneProfeb.transform.FindChild("SceneObjects/Events/Events_04");
                if (trEvent4 != null) FightScene.s_InstanceThis.PushEventPos(trEvent4.position);
                Transform trEvent5 = sceneProfeb.transform.FindChild("SceneObjects/Events/Events_05");
                if (trEvent5 != null) FightScene.s_InstanceThis.PushEventPos(trEvent5.position);
                Transform trEvent6 = sceneProfeb.transform.FindChild("SceneObjects/Events/Events_06");
                if (trEvent6 != null) FightScene.s_InstanceThis.PushEventPos(trEvent6.position);

                Transform condition = sceneProfeb.transform.FindChild("Condition/00");
                if (condition != null) FightScene.s_InstanceThis.SetConditionPos(condition.position);

                // NPC/00   ....
                Transform trNpcs = sceneProfeb.transform.FindChild("NPC");
                if (trNpcs != null)
                {
                    for (int n = 0; n < trNpcs.childCount; n++)
                    {
                        Transform tr = trNpcs.GetChild(n);
                        FightScene.s_InstanceThis.PushNPCPos(tr.position);
                    }
                }

                // 中立物件 
                Transform trNeutrals = sceneProfeb.transform.FindChild("NeutralObjects");
                if (trNeutrals != null)
                {
                    for (int n = 0; n < trNeutrals.childCount; n++)
                    {
                        Transform tr = trNeutrals.GetChild(n);
                        FightScene.s_InstanceThis.PushNeutralPos(tr.position);
                    }
                }

                // Totems  预置战场 00
                Transform trt = sceneProfeb.transform.FindChild("SceneObjects/Totems/00");
                if (trt != null)
                {
                    FightScene.s_InstanceThis.SetSceneTotemPos(trt.position);
                }

                // 传送点
                Transform trss = sceneProfeb.transform.FindChild("SceneObjects/Transfer/Start");
                if (trss != null)
                {
                    for (int n = 0; n < trss.childCount; n++)
                    {
                        Transform tr = trss.GetChild(n);
                        FightScene.s_InstanceThis.PushStartPos(tr.position);
                    }
                }
                Transform tres = sceneProfeb.transform.FindChild("SceneObjects/Transfer/End");
                if (tres != null)
                {
                    for (int n = 0; n < tres.childCount; n++)
                    {
                        Transform tr = tres.GetChild(n);
                        FightScene.s_InstanceThis.PushEndPos(tr.position);
                    }
                }


                if (initUI)
                {
                    //create camera
                    Object ui_prefab = Resources.Load("UI/Prefabs/FightSceneUI/UI");
                    GameObject ui_go = GameObject.Instantiate(ui_prefab) as GameObject;
                    ui_go.name = "UI";
                    ui_go.transform.parent = scene_go.transform;

                    Transform trGround = sceneProfeb.transform.FindChild("Ground/0");
                    ui_go.transform.FindChild("PathGridWithObstacles").position = trGround.position;

                    Transform trCam = sceneProfeb.transform.FindChild("Cam");
                    ui_go.transform.position = trCam.position;
                    ui_go.transform.rotation = trCam.rotation;
                }
            }

            // Create Over
            return scene_go;
        }
        else
        {
            Debug.LogError("Load MAP " + sceneName + "NULL!");

            string json = IOTool.LoadJson("Scene/" + sceneName + ".json");
            SceneConf conf_scene = new SceneConf(IOTool.DeserializeObject(json));// IOTool.DeserializeObject<SceneConf>(json);
            if (conf_scene == null)
            {
                Debug.LogError("Load Json ERROR!");
                return null;
            }
            if (conf_scene.Bounds == null)
            {
                Debug.LogError("Load Json Bounds NULL!");
                LoadScene(conf_scene);
                return null;
            }
            // 应该在外部工具存储的时候转为世界坐标存储
            GameObject scene = LoadScene(conf_scene);
            Transform parent = scene.transform.FindChild("Map/Bounds");
            foreach (var item in conf_scene.Bounds)
            {
                // 相对坐标转世界坐标
                _cur_scene_bounds.Add(parent.localToWorldMatrix.MultiplyPoint(item.GetPosition()));
            }
            return scene;
        }
    }

    /// <summary>
    /// 加载场景 
    /// </summary>
    /// <param name="sceneName">例如： Scene_1_1 酱紫 </param>
    public GameObject LoadScene(SceneConf conf_scene) 
    {
        GameObject scene_go = GameObject.Find("Scene");

        if (scene_go == null) {
            Debug.LogError("导入场景数据出错，场景中无Scene节点！");
            return null;
        }

        //create light
        Object light_prefab = Resources.Load("UI/Prefabs/" + conf_scene.Light.PrefabName);
        GameObject light_go = GameObject.Instantiate(light_prefab) as GameObject;
        light_go.name = conf_scene.Light.Name;
        light_go.transform.localPosition = conf_scene.Light.GetPosition();
        light_go.transform.localRotation = conf_scene.Light.GetRotation();
        light_go.transform.localScale = conf_scene.Light.GetScale();
        light_go.transform.parent = scene_go.transform;

        //create camera
        Object ui_prefab = Resources.Load("UI/Prefabs/FightSceneUI/" + conf_scene.UI.PrefabName);
        GameObject ui_go = GameObject.Instantiate(ui_prefab) as GameObject;
        ui_go.name = conf_scene.UI.PrefabName;
        GameObject camera_go = ui_go.transform.FindChild("CameraScene").gameObject;
        camera_go.transform.localPosition = conf_scene.UI.GetPosition();
        camera_go.transform.localRotation = conf_scene.UI.GetRotation();
        camera_go.transform.localScale = conf_scene.UI.GetScale();
        ui_go.transform.parent = scene_go.transform;


        GameObject camera_ui = ui_go.transform.FindChild("Camera").gameObject;
        camera_ui.transform.localRotation = conf_scene.UI.GetRotation();

        /*
        //create scene
        scene_go = new GameObject("Scene");
        scene_go.name = conf_scene.Name;
        scene_go.transform.localPosition = conf_scene.GetPosition();
        scene_go.transform.localRotation = conf_scene.GetRotation();
        scene_go.transform.localScale = conf_scene.GetScale();
        */
        //create map
        GameObject map_go = new GameObject(conf_scene.Map.Name);
        map_go.transform.parent = scene_go.transform;
        map_go.transform.localPosition = conf_scene.Map.GetPosition();
        map_go.transform.localRotation = conf_scene.Map.GetRotation();
        map_go.transform.localScale = conf_scene.Map.GetScale();

        //create bounds
        GameObject bounds_root_go = new GameObject("Bounds");
        Object point_prefab = Resources.Load("UI/Prefabs/Point_10");
        bounds_root_go.transform.parent = map_go.transform;
        bounds_root_go.transform.localRotation = Quaternion.identity;
        bounds_root_go.transform.localScale = Vector3.one;
        bounds_root_go.transform.localPosition = Vector3.zero;


        if (conf_scene.Bounds != null)
        {
            foreach (var item in conf_scene.Bounds)
            {
                GameObject point_go = GameObject.Instantiate(point_prefab) as GameObject;
                point_go.name = item.Name;
                point_go.transform.parent = bounds_root_go.transform;
                point_go.transform.localPosition = item.GetPosition();
                point_go.transform.localRotation = item.GetRotation();
                point_go.transform.localScale = item.GetScale();
            }
        }

        //create ground
        GameObject ground_go = new GameObject(conf_scene.Map.Ground.Name);
        ground_go.transform.parent = map_go.transform;
        ground_go.transform.localPosition = conf_scene.Map.Ground.GetPosition();
        ground_go.transform.localRotation = conf_scene.Map.Ground.GetRotation();
        ground_go.transform.localScale = conf_scene.Map.Ground.GetScale();

        if (conf_scene.PlaceHolder != null)
        {
            foreach (var info in conf_scene.PlaceHolder)
            {
                //Debug.Log("位置点信息： " + info.GetPosition());
                FightScene.s_InstanceThis.PushPos(info.GetPosition());
            }
        }
        else
        {
            Debug.Log("位置点信息为空");
        }
        
        Mesh ground_mesh = PlaneInfo.CreateMesh((float)conf_scene.Map.Ground.CellWidth * 0.5f, (float)conf_scene.Map.Ground.CellHeight * 0.5f);
        foreach (var cell in conf_scene.Map.Ground.GroundCells)
        {
            GameObject cell_go;
            Object res = Resources.Load("Materials/Map/" + cell.Materials[0]);
            if(res != null)
            {
                Material matX = res as Material;
                cell_go = PlaneInfo.CreatePlaneWithMesh(cell.GetPosition(), ground_mesh, cell.Name, ground_go.transform, matX);
            }
            else
            {
                Debug.LogError("NotFound " + cell.Materials[0]);
                cell_go = PlaneInfo.CreatePlaneWithMesh(cell.GetPosition(), ground_mesh, cell.Name, ground_go.transform, null);
                if (cell.Materials.Count > 0)
                {
                    FightScene.s_InstanceThis.m_MBM.AddMaterial(cell_go, cell.Materials[0]);
                }
            }
            cell_go.transform.localRotation = cell.GetRotation();
            cell_go.transform.localScale = cell.GetScale();
        }

        //create surface
        GameObject surface_go = new GameObject(conf_scene.Map.Surface.Name);
        surface_go.transform.parent = map_go.transform;
        surface_go.transform.localPosition = conf_scene.Map.Surface.GetPosition();
        surface_go.transform.localRotation = conf_scene.Map.Surface.GetRotation();
        surface_go.transform.localScale = conf_scene.Map.Surface.GetScale();

        if (conf_scene.Map.Surface.Planes != null) {
            foreach (var item in conf_scene.Map.Surface.Planes)
            {
                GameObject item_go;
                Object res = Resources.Load("Materials/Map/" + item.Materials[0]);
                if(res != null)
                {
                    Material mat = res as Material;
                    item_go = PlaneInfo.CreatePlane((float)item.CellWidth * 0.5f, (float)item.CellHeight * 0.5f, item.Name, surface_go.transform, mat);
                }
                else
                {
                    Debug.LogError("NotFound " + item.Materials[0]);
                    item_go = PlaneInfo.CreatePlane((float)item.CellWidth * 0.5f, (float)item.CellHeight * 0.5f, item.Name, surface_go.transform, null);
                    if (item.Materials.Count > 0)
                    {
                        FightScene.s_InstanceThis.m_MBM.AddMaterial(item_go, item.Materials[0]);
                    }
                }
                item_go.transform.localPosition = item.GetPosition();
                item_go.transform.localRotation = item.GetRotation();
                item_go.transform.localScale = item.GetScale();
            }
        }

        if (conf_scene.Map.Surface.Colliders != null) {
            foreach (var item in conf_scene.Map.Surface.Colliders) {
                GameObject go = new GameObject(item.Name);
                go.transform.parent = surface_go.transform;
                go.transform.localPosition = item.GetPosition();
                go.transform.localRotation = item.GetRotation();
                go.transform.localScale = item.GetScale();
                BoxCollider comp = go.AddComponent<BoxCollider>();
                comp.center = item.ListToVector3(item.Center);
                comp.size = item.ListToVector3(item.Size);
            }
        }

        if (conf_scene.Map.Surface.PlaneWithColliders != null) {
            foreach (var item in conf_scene.Map.Surface.PlaneWithColliders)
            {
                GameObject item_go;
                Object res = Resources.Load("Materials/Map/" + item.Plane.Materials[0]);
                if (res != null)
                {
                    Material mat = res as Material;
                    item_go = PlaneInfo.CreatePlane((float)item.Plane.CellWidth * 0.5f, (float)item.Plane.CellHeight * 0.5f, item.Name, surface_go.transform, mat);
                }
                else
                {
                    Debug.LogError("NotFound " + item.Plane.Materials[0]);
                    item_go = PlaneInfo.CreatePlane((float)item.Plane.CellWidth * 0.5f, (float)item.Plane.CellHeight * 0.5f, item.Name, surface_go.transform, null);
                    if (item.Plane.Materials.Count > 0)
                    {
                        FightScene.s_InstanceThis.m_MBM.AddMaterial(item_go, item.Plane.Materials[0]);
                    }
                }
                item_go.transform.localPosition = item.GetPosition();
                item_go.transform.localRotation = item.GetRotation();
                item_go.transform.localScale = item.GetScale();

                foreach (var item_collider in item.Colliders) {
                    GameObject go = new GameObject(item_collider.Name);
                    go.transform.parent = item_go.transform;
                    go.transform.localPosition = item_collider.GetPosition();
                    go.transform.localRotation = item_collider.GetRotation();
                    go.transform.localScale = item_collider.GetScale();
                    BoxCollider comp = go.AddComponent<BoxCollider>();
                    comp.center = item_collider.ListToVector3(item_collider.Center);
                    comp.size = item_collider.ListToVector3(item_collider.Size);
                }
            }
        }
        // Create Over
        return scene_go;
    }
}