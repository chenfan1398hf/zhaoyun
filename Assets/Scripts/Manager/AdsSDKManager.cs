using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���������ռ�
using UnityEngine.Events;

namespace Scripts.SDK
{
    public class AdsSDKManager
    {
        #region AdsSDKManager
        public readonly static AdsSDKManager Instance = new AdsSDKManager();
        private System.Action mAction;
        //private bool soundOn = false;
        public bool bInitSdk = false;
        //private string DeviceID = "";
        #endregion

        #region InitSDK ��ʼ��SDK
        public void InitSDK(System.Action action)
        {
            bInitSdk = false;
            mAction = action;
            try
            {
                
            }
            catch (System.DivideByZeroException e)
            {
                Debug.LogErrorFormat("Exception InitSDK: {0}", e);
            }
            finally
            {

            }

        }
        #endregion

        #region initListener ��ʼ������
        public void initListener()
        {
            try
            {

            }
            catch (System.DivideByZeroException e)
            {
                Debug.LogErrorFormat("Exception initListener: {0}", e);
            }
            finally
            {

            }

        }
        #endregion


#region �������


        //private bool isLoadAd = false;
        //private bool rewardAdVerify = false;
        //private bool isAutoPlayAD = false;
       // private LightGameRewardAd _rewardAd;
        public void initRewardAd()
        {

            CreateRewardVideoAd();
        }

        public void CreateRewardVideoAd() {

        }
        public void LoadAd()
        {
          
        }

        public void ShowAdWithSenseInfo()
        {
 
        }
        public void ShowRewardAd()
        {


        }
        private void ShowAd()
        {
     
        }

        public void ReleaseAdObject()
        { 
        
        }
#endregion
    }
}
