using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Gamekit2D
{
    [RequireComponent(typeof(Collider2D))]
    public class DirectorTrigger : MonoBehaviour, IDataPersister
    {
        public enum TriggerType
        {
            Once, Everytime,
        }

        [Tooltip("This is the gameobject which will trigger the director to play.  For example, the player.")]
        public GameObject triggeringGameObject;
        public PlayableDirector director;
        public TriggerType triggerType;
        public UnityEvent OnDirectorPlay;
        public UnityEvent OnDirectorFinish;
        [HideInInspector]
        public DataSettings dataSettings;

        protected bool m_AlreadyTriggered;

        void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        //碰撞事件
        void OnTriggerEnter2D(Collider2D other)
        {
            //如果与玩家发生碰撞
            if (other.gameObject != triggeringGameObject)
                return;
            //如果之前已经发生碰撞
            if (triggerType == TriggerType.Once && m_AlreadyTriggered)
                return;
            //播放相机动画
            director.Play();
            m_AlreadyTriggered = true;
            //剥夺玩家对Player的控制
            OnDirectorPlay.Invoke();
            //经过相机动画之后，执行Invoke
            Invoke("FinishInvoke", (float)director.duration);
        }

        void FinishInvoke()
        {
            //返还给玩家控制权
            OnDirectorFinish.Invoke();
        }

        public void OverrideAlreadyTriggered(bool alreadyTriggered)
        {
            m_AlreadyTriggered = alreadyTriggered;
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<bool>(m_AlreadyTriggered);
        }

        public void LoadData(Data data)
        {
            Data<bool> directorTriggerData = (Data<bool>)data;
            m_AlreadyTriggered = directorTriggerData.value;
        }
    }
}