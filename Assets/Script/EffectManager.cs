using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("References")]
    public static EffectManager instance;

    #region Class
    // ���� ����Ʈ ���� ����
    [System.Serializable]
    public class Effect
    {
        public string _name;                // ����Ʈ �̸� (�ĺ���)
        public GameObject _effectPrefab;    // ����Ʈ ������
        public int _initialPoolSize = 5;    // �ʱ� Ǯ ũ��
    }

    // Ǯ�� ���� ��Ÿ�� ������
    public class EffectPoolData
    {
        public Queue<GameObject> poolQueue; // ��� ���� ����Ʈ ������Ʈ
        public int currentSize;             // ���� Ǯ�� ������ ��ü ����

        public EffectPoolData(int initial)
        {
            poolQueue = new Queue<GameObject>();
            currentSize = initial;
        }
    }

    #endregion

    #region References
    public Effect[] _effects; // �ν����Ϳ��� ������ ����Ʈ ���
    private Dictionary<string, EffectPoolData> _effectPools; // ����Ʈ �̸� �� Ǯ ���� ����

    #endregion

    #region Unity CallBack Functions
    private void Awake()
    {
        // �̱��� ���� ���� �� �ʱ�ȭ
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı� ����
            InitializeEffectPools();       // ����Ʈ Ǯ �ʱ�ȭ
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Effect
    // ��� ����Ʈ Ǯ�� �ʱ�ȭ
    private void InitializeEffectPools()
    {
        _effectPools = new Dictionary<string, EffectPoolData>();

        foreach (var effect in _effects)
        {
            var poolData = new EffectPoolData(effect._initialPoolSize);

            // �ʱ� Ǯ ũ�⸸ŭ ���� �� ��Ȱ��ȭ
            for (int i = 0; i < effect._initialPoolSize; i++)
            {
                GameObject obj = CreateNewInstance(effect._effectPrefab);
                poolData.poolQueue.Enqueue(obj);
            }

            _effectPools.Add(effect._name, poolData);
        }
    }

    // ����Ʈ �ν��Ͻ��� �����ϰ� ��Ȱ��ȭ�� ���·� ��ȯ
    private GameObject CreateNewInstance(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab);
        instance.SetActive(false);
        instance.transform.SetParent(transform); // ���� ������ �θ� ����
        return instance;
    }

    // ��ġ�� ȸ���� �����ϰ� ����Ʈ ������Ʈ�� Ȱ��ȭ
    private void SetupEffectInstance(GameObject instance, Vector3 position, Quaternion rotation)
    {
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.SetActive(true);
    }

    // �ܺο��� ȣ��Ǵ� ����Ʈ ��� �Լ�
    public void PlayEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        if (!_effectPools.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect not found: {effectName}");
            return;
        }

        EffectPoolData poolData = _effectPools[effectName];
        GameObject effectInstance;

        // ť�� �����ִ� ������Ʈ ���, ������ ���� ����
        if (poolData.poolQueue.Count > 0)
        {
            effectInstance = poolData.poolQueue.Dequeue();
        }
        else
        {
            var effectPrefab = _effects.First(e => e._name == effectName)._effectPrefab;
            effectInstance = CreateNewInstance(effectPrefab);
            poolData.currentSize++;
        }

        SetupEffectInstance(effectInstance, position, rotation);
        StartCoroutine(ReturnToPoolAfterDelay(effectInstance, effectName));     // �ڵ� ��ȯ ó��
    }

    // ����Ʈ ��� �� ���� �ð� �ڿ� Ǯ�� ��ȯ
    private IEnumerator ReturnToPoolAfterDelay(GameObject instance, string effectName)
    {
        if (instance == null) yield break;

        // ��ƼŬ ���� �ð� ���
        ParticleSystem particle = instance.GetComponent<ParticleSystem>();
        float duration = particle != null ? particle.main.duration : 1f;

        yield return new WaitForSeconds(duration + 0.5f);                   // ���� �ð� ����

        if (instance != null && _effectPools.ContainsKey(effectName))
        {
            instance.SetActive(false);
            _effectPools[effectName].poolQueue.Enqueue(instance);           // �ٽ� Ǯ�� ��ȯ
        }
    }
  
    #endregion
}