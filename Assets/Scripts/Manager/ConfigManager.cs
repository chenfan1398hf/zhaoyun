using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.Linq;
using System.IO;

public class ConfigManager
{

    public List<TaskInfoCfg> TaskInfoCfg = new List<TaskInfoCfg>();                             //任务配置
    //读取游戏配置表
    public void InitGameCfg()  //初始化自定义的游戏配置表
    {
        //故意未Try 这部分加载 如果配置表异常会直接关闭程序
        //string txt = FileTool.Read_Txt("taskInfo");
        //TaskInfoCfg = JsonMapper.ToObject<List<TaskInfoCfg>>(txt);

        return;
    }

    //public LanguageCfg GetLanguageCfgByKey(string key)
    //{
    //    return mLanguageCfg.Find((item) => item.key == key);
    //}
}

public class GameData
{
    public int Hp;
    public int MaxHp;
    public int Attack;
    public int Type;            //1英雄 2Boss
    public bool Live = true;          //true活着 false 死亡

    public void AddHp(int _number)
    {
        Hp += _number;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
        if (Hp <= 0)
        {
            Hp = 0;
            Live = false;
        }
    }
    
}