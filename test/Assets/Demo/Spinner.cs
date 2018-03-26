using System.Collections;
using N.Packages.Promises;
using UnityEngine;

namespace Assets.Demo
{
  public class Spinner : MonoBehaviour
  {
    public bool StartSpin;

    public void Update()
    {
      if (StartSpin)
      {
        StartSpin = false;
        AsyncWorker.Run(SpinCube).Then(() => { Debug.Log("Spin finished!!"); }, Debug.LogException).Dispatch();
      }
    }

    private IEnumerator SpinCube()
    {
      for (var i = 0; i < 100; i++)
      {
        Debug.Log($"{i}");      
        transform.rotation *= Quaternion.AngleAxis(5, Vector3.up);
        yield return new WaitForSeconds(0.1f);
      }
    }
  }
}