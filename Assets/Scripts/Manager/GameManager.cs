using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;

public class GameManager :MonoSingleton<GameManager>
{
    #region 构造函数及其变量
    public GameManager()
    {
        configMag = new ConfigManager();
    }
    public static bool isDbugLog = true;
    public PlayerData playerData = null;                            //玩家数据（本地持久化）
    public ConfigManager configMag;
    private System.Random Random;                                   //随机种子
    private int TimeNumber = 0;
    private List<UnityAction> unityActionList = new List<UnityAction>();
    public bool isBattle = true;


    public static int TI_LI_MAX_NUMBER = 100;
    public static int TI_LI_CD_NUMBER = 600;

    #endregion

    private void Update()
    {
        foreach (var item in unityActionList)
        {
            item.Invoke();
        }
    }
    #region Awake()
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;//设置帧率为60帧
        GetLocalPlayerData();
        Random = new System.Random(Guid.NewGuid().GetHashCode());
    }
    #endregion



    private void Start()
    {
        this.InvokeRepeating("CheckTime", 0, 0.1f);
        InitData();
        BeginGame();
    }

    void CheckTime()
    {
        TimeNumber++;

        if (TimeNumber % 10 == 0)
        {
            ShowRenwu();
        }
        if (TimeNumber % 20 == 0)
        {

        }


    }


    #region OnApplicationPause(bool pause)切屏感知
    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            if (isDbugLog)
                Debug.Log("切屏感知");
            SaveGame();
        }
    }
    #endregion

    #region OnApplicationQuit() 退出游戏感知
    public void OnApplicationQuit()
    {
        if (isDbugLog)
            Debug.Log("退出感知");
        SaveGame();

    }
    #endregion

    #region 获取本地数据
    public void GetLocalPlayerData()
    {
        playerData = PlayerData.GetLocalData();//读取本地持久化玩家数据(包括本土化设置)
        configMag.InitGameCfg();//读取配置表
        playerData.InitData();//根据配置表和本地数据初始化游戏数据
    }
    #endregion

    #region SaveGame() 保存玩家数据
    public void SaveGame()
    {
        //if(SocketManager.instance.socket!=null)
        //SocketManager.instance.socket.Disconnect();
        playerData.Save();
    }
    #endregion

    #region OnDestroy()
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    #endregion

    /// <summary>
    /// 注册一个update在这里跑
    /// </summary>
    /// <param name="_action"></param>
    public void AddUpdateListener(UnityAction _action)
    {
        unityActionList.Add(_action);
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImage(string id, Image image)
    {
        string path = "Icon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片--装备图标
    /// </summary>
    public void SpritPropEquipIcon(string id, Image image)
    {
        string path = "EquipIcon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }


    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, Image image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, SpriteRenderer image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 添加预制体
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fatherTransform"></param>
    /// <returns></returns>
    public GameObject AddPrefab(string name, Transform fatherTransform)
    {
        string newpath = "Prefab/" + name;
        GameObject obj = ObjPool.instance.GetObj(newpath, fatherTransform);
        obj.AddComponent<DesObj>();
        obj.GetComponent<DesObj>().InitDes(newpath);
        return obj;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(string name, GameObject gameObject)
    {
        string[] list = name.Split(new char[] { '(' });
        if (list.Length != 2)
        {
            string newpath = "Prefab/" + name;
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        else
        {
            string newpath = "Prefab/" + list[0];
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject prefabObj, GameObject gameObject, string _path = null)
    {
        ObjPool.instance.Recycle(prefabObj, gameObject, "Prefab/" + _path);
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject gameObject)
    {
        string name = gameObject.GetComponent<DesObj>().name;
        ObjPool.instance.Recycle(name, gameObject);
        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(SkeletonGraphic _skeletonGraphic, bool isLoop, string _spineName, bool isRest)
    {
        if (isRest)
        {
            _skeletonGraphic.AnimationState.ClearTracks();
            _skeletonGraphic.AnimationState.Update(0);
        }
        _skeletonGraphic.AnimationState.SetAnimation(0, _spineName, isLoop);

        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(Animator _animator, string _spineName, bool isRest)
    {
        //_animator.Play(_spineName, 0 ,0f);
        if (isRest)
        {
            //_animator.Update(0);
            _animator.Play(_spineName, 0, 0f);
        }
        else
        {
            _animator.Play(_spineName);
        }
        return;
    }
    /// <summary>
    /// 获取对象池内对象数据
    /// </summary>
    /// <returns></returns>
    public ObjPool.PoolItem GetPoolItem(string name)
    {
        string newpath = "Prefab/" + name;
        return ObjPool.instance.GetPoolItem(newpath); ;
    }
    /// <summary>
    /// 网络拉取图片
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_image"></param>
    /// <returns></returns>
    public IEnumerator GetHead(string _url, Image _image)
    {
        if (_url == string.Empty || _url == "")
        {
            _url = "https://p11.douyinpic.com/aweme/100x100/aweme-avatar/mosaic-legacy_3797_2889309425.jpeg?from=3067671334";
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1f, 1f));
                _image.sprite = sprite;
                //Renderer renderer = plane.GetComponent<Renderer>();
                //renderer.material.mainTexture = texture;
            }
        }
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    public void CleraPlayerData()
    {
        PlayerData.ClearLocalData();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Editor/Tools/Clear")]
    static void CleraPlayerData1()
    {
        PlayerData.ClearLocalData();
    }
#endif
    private GameObject[] GetDontDestroyOnLoadGameObjects()
    {
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        //移除所有场景包含的对象
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //移除父级不为null的对象
        int k = allGameObjects.Count;
        while (--k >= 0)
        {
            if (allGameObjects[k].transform.parent != null)
            {
                allGameObjects.RemoveAt(k);
            }
        }
        return allGameObjects.ToArray();
    }

    public GameObject playerObj;
    private GameObject map1;
    private GameObject map2;
    private GameObject map3;
    public GameObject beginPanel;
    public GameObject gamePanel;
    public GameObject endPanel;
    public GameObject taskPanel;
    public GameObject nextPanel;
    public bool isBeginLevel = false;
    private MusicManager musicManager;
    public List<GameObject> bossList = new List<GameObject>();

    //初始化数据
    public void InitData()
    {
        map1 = GameObject.Find("Map1").gameObject;
        map2 = GameObject.Find("Map2").gameObject;
        map3 = GameObject.Find("Map3").gameObject;
        endPanel.SetActive(false);
        taskPanel.SetActive(false);
        nextPanel.SetActive(false);
        musicManager = new MusicManager();
    }
    //开始游戏
    public void BeginGame()
    {
        beginPanel.SetActive(true);
        isBeginLevel = true;
        musicManager.PlayBkMusic("开始界面音乐");
        ChangeWq(1);
    }

    //开始关卡
    public void BeginLevel()
    {
        playerData.playerLevel = 1;
        ChangeLevel();
        beginPanel.SetActive(false);
        musicManager.PlayBkMusic("背景音乐");
    }
    public void BeginLevel2()
    {
        ChangeLevel();
        beginPanel.SetActive(false);
        musicManager.PlayBkMusic("背景音乐");
    }
    //关卡跳转
    public void ChangeLevel()
    {
        Vector3 vector3 = Vector3.zero;
        if (playerData.playerLevel == 1)
        {
            map1.gameObject.SetActive(true);
            map2.gameObject.SetActive(false);
            map3.gameObject.SetActive(false);
            vector3 = map1.transform.Find("biaoji").transform.position;

        }
        else if (playerData.playerLevel == 2)
        {
            map1.gameObject.SetActive(false);
            map2.gameObject.SetActive(true);
            map3.gameObject.SetActive(false);
            vector3 = map2.transform.Find("biaoji").transform.position;
            ChangeWq(2);
            GameManager.instance.AddTask(3);
            var obj = playerObj.GetComponent<AdvancedPlayerController>();
            obj.heroGameData.AddHp(obj.heroGameData.MaxHp);
            GameManager.instance.UpdateHeroHp(obj.heroGameData);
        }
        else if (playerData.playerLevel == 3)
        {
            map1.gameObject.SetActive(false);
            map2.gameObject.SetActive(false);
            map3.gameObject.SetActive(true);
            vector3 = map3.transform.Find("biaoji").transform.position;
            ChangeWq(2);
            GameManager.instance.AddTask(4);
            var obj = playerObj.GetComponent<AdvancedPlayerController>();
            obj.heroGameData.AddHp(obj.heroGameData.MaxHp);
            GameManager.instance.UpdateHeroHp(obj.heroGameData);
        }
        playerObj.transform.position = vector3;
    }
    //打开结束面板
    public void OpenEndPanel()
    {
        endPanel.SetActive(!endPanel.activeSelf);
    }
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    public void ChangeAudio(float _value)
    {
        musicManager.ChangeBkValue(_value);
        musicManager.ChangeSoundValue(_value);
    }
    //刷新显示英雄血量
    public void UpdateHeroHp(GameData _gameData)
    {
        gamePanel.transform.Find("Image/Hp/Hp").GetComponent<Image>().fillAmount = (float)_gameData.Hp / (float)_gameData.MaxHp;
        gamePanel.transform.Find("Image/Hp/HpNuber").GetComponent<Text>().text = _gameData.Hp.ToString();
    }
    //刷新任务
    public void ShowRenwu()
    {
        gamePanel.transform.Find("Image1/Text (Legacy) (1)").GetComponent<Text>().text = GetRenwuMsg();
    }
    public string GetRenwuMsg()
    {
        string msg = string.Empty;
        if (playerData.playerRenWuIndex == 1)
        {
            msg = "找NPC接取第一个任务";
        }
        else if (playerData.playerRenWuIndex == 2)
        {
            msg = "击败山贼，保卫村庄,任务奖励：武器龙胆亮银枪";
        }
        else if (playerData.playerRenWuIndex == 3)
        {
            msg = "博望坡之战初显身手，击败夏侯惇。";
        }
        else if (playerData.playerRenWuIndex == 4)
        {
            msg = "单骑七进七出救阿斗，击败夏侯恩。";
        }
        return msg;
    }
    public void OpenTaskPanel(bool isBool)
    {
        taskPanel.SetActive(isBool);
    }
    public void AddTask(int _number)
    {
        playerData.playerRenWuIndex = _number;
        ShowRenwu();
    }
    public bool CheckBossDeath()
    {
        int maxNumber = 0;
        int liveNumber = 0;
        for (int i = 0; i < bossList.Count; i++)
        {
            if (bossList[i].gameObject.GetComponent<Boss>().LevelNumber == playerData.playerLevel)
            {
                maxNumber++;
            }
        }
        for (int i = 0; i < bossList.Count; i++)
        {
            if (bossList[i].gameObject.GetComponent<Boss>().BossGameData.Live == false)
            {
                liveNumber++;
            }
        }
        if (liveNumber >= maxNumber)
        {
            return true;
        }
        return false;
    }
    //前往下一关
    public void NextLevel()
    {
        playerData.playerLevel++;
        ChangeLevel();
        nextPanel.SetActive(false);
    }
    //任务奖励武器切换
    public GameObject wuqiobj1;
    public GameObject wuqiobj2;
    public void ChangeWq(int value)
    {
        if (value == 1)
        {
            wuqiobj1.SetActive(true);
            wuqiobj2.SetActive(false);
        }
        else
        {
            wuqiobj1.SetActive(false);
            wuqiobj2.SetActive(true);
        }
    }
}
