using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : BaseManager<MusicManager>
{
    
    private const string musicPath = "Music/";
    private AudioSource bkMusic = null;
    private float bkValue = 1f;

    private GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float yxValue = 1f;

    public MusicManager()
    {
        GameManager.instance.AddUpdateListener(CheckUpdate);
    }
    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="_name">������</param>
    public void PlayBkMusic(string _name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BkMusic";
            bkMusic = obj.AddComponent<AudioSource>();
        }
        //�������ֽϴ��첽���ر�������
        GameManager.instance.StartCoroutine(LoadBkMusic(bkMusic, musicPath+ _name));
    }

    IEnumerator LoadBkMusic(AudioSource _audio, string _path)
    {
        ResourceRequest req = Resources.LoadAsync<AudioClip>(_path);
        yield return req;

        _audio.clip = req.asset as AudioClip;
        _audio.volume = bkValue;
        _audio.loop = true;
        _audio.Play();

    }
    /// <summary>
    /// ��ͣ��������
    /// </summary>
    public void PauseBkMusic()
    {
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.Pause();
    }
    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopBkMusic()
    {
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.Stop();
    }
    /// <summary>
    /// �ı�������С
    /// </summary>
    /// <param name="_value"></param>
    public void ChangeBkValue(float _value)
    {
        bkValue = _value;
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.volume = bkValue;
    }




    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="_name"></param>
    public void PlaySound(string _name)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";
        }
        var audio = soundObj.AddComponent<AudioSource>();
        AudioClip clip = ResourcesLoad.instance.Load<AudioClip>(musicPath + _name);
        audio.clip = clip;
        audio.volume = yxValue;
        audio.loop = false;
        audio.Play();

        soundList.Add(audio);
    }
    /// <summary>
    /// �ر���Ч
    /// </summary>
    /// <param name="_audioSource"></param>
    public void StopSound(AudioSource _audioSource)
    {
        if (soundList.Contains(_audioSource))
        {
            soundList.Remove(_audioSource);
            _audioSource.Stop();
            GameObject.Destroy(_audioSource);
        }
    }
    /// <summary>
    /// �ı���Ч��С
    /// </summary>
    /// <param name="_value"></param>
    public void ChangeSoundValue(float _value)
    {
        yxValue = _value;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = yxValue;
        }

    }
    /// <summary>
    /// ���������Ч�Ƴ�
    /// </summary>
    private void CheckUpdate()
    {
        for (int i = soundList.Count -1; i >= 0; --i)
        {
            if (! soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }
}
