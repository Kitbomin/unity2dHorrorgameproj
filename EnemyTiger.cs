using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EnemyTiger : MonoBehaviour
{
    private EnemyManager enemyManager;
    public int Life;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        enemyManager = GetComponent<EnemyManager>();
        if (enemyManager == null)
        {
            Debug.LogWarning("enemyManager이 없습니다.");
        }
    }

    public void DecreaseLife(int amount)
    {
        Life -= amount;
        Debug.Log($"Enemy life: {Life}");


        if (Life <= 0)
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
