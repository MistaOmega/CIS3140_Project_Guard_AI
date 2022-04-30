using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class GuardManagerHolder : MonoBehaviour
{
    private float waitTime = 1f;
    public static GuardManagerHolder Instance { get; private set; }

    public List<GuardStateMan> guards;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        if (Instance == null)
        {
            GameObject objectOfType = (GameObject)FindObjectOfType(typeof(GuardManagerHolder));
            Instance = objectOfType.GetComponent<GuardManagerHolder>();
        }
        FindGuards();
    }

    public void FindGuards()
    {
        GuardStateMan[] myItems = FindObjectsOfType(typeof(GuardStateMan)) as GuardStateMan[];
        //Debug.Log("Found " + myItems.Length + " instances with this script attached");
        if (myItems == null) return;
        foreach (GuardStateMan item in myItems)
        {
            if (!guards.Contains(item))
            {
                guards.Add(item);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Instance == null)
        {
            GameObject objectOfType = (GameObject)FindObjectOfType(typeof(GuardManagerHolder));
            Instance = objectOfType.GetComponent<GuardManagerHolder>();
        }
        if (waitTime < 0.1f)
        {
            FindGuards();
            waitTime = 1f;
        }

        waitTime -= Time.deltaTime;
    }
}
