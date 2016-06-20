using UnityEngine;
using RAIN.Navigation.NavMesh;
using System.Threading;
using System.Collections;

public class NavaigationManager : MonoBehaviour
{

    [SerializeField]
    private int _threadCount = 4;

    // Use this for initialization
    void Start()
    {
        GenerateNavmesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateNavmesh()
    {
        NavMeshRig tRig = GetComponent<NavMeshRig>();

        // Unregister any navigation mesh we may already have (probably none if you are using this)
        tRig.NavMesh.UnregisterNavigationGraph();

        tRig.NavMesh.StartCreatingContours(tRig, _threadCount);
        while (tRig.NavMesh.Creating)
        {
            tRig.NavMesh.CreateContours();

            // This could be changed to a yield (and the function to a coroutine) although as 
            // of RAIN of 2.1.4.0 (fixed since then), our navigation mesh editor has issues with it
            Thread.Sleep(10);
        }

        tRig.NavMesh.RegisterNavigationGraph();
    }
}
