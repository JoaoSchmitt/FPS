using UnityEngine;

public class Generic : MonoBehaviour
{
    public static GameObject GetClosestObject(Transform reference, GameObject[] objects)
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = reference.position;
        foreach (GameObject t in objects)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    public static void StartFadeOutSound(AudioSource sound, float fadeSpeed)
    {
        do
        {
            sound.volume -= fadeSpeed * Time.deltaTime;
        }
        while (sound.volume > 0);

        if (sound.volume <= 0)
        {
            sound.Stop();
            sound.volume = 100;
        }
    }
}
