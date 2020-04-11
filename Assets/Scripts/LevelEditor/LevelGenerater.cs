using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerater : MonoBehaviour
{
    [SerializeField] List<GameObject> levels = null;
    [SerializeField] Player player = null;
    [SerializeField] Camera mainCamera;
    private int currentLevelIndex;
    private int prevLevelIndex;

    // Start is called before the first frame update
    void Start()
    {
        currentLevelIndex = 0;
        prevLevelIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        LevelCanBeGenerated();
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        if (LevelCanBeGenerated())
        {
            prevLevelIndex = currentLevelIndex;
            do
            {
                currentLevelIndex = Random.Range(0, levels.Count);
            } while (prevLevelIndex == currentLevelIndex);
            SpawnLevel(levels[currentLevelIndex], levels[prevLevelIndex].GetComponent<LevelEditor>().levelEndPosition);
        }
    }

    private bool LevelCanBeGenerated()
    {
        if (Mathf.Abs(player.transform.position.x - levels[currentLevelIndex].GetComponent<LevelEditor>().levelEndPosition.position.x) <= 10f)
        {
            return true;
        }
        return false;
    }

    private void SpawnLevel(GameObject level, Transform levelPosition)
    {
        levelPosition.position += GetCameraHalfSizeVector();
        Instantiate(level, levelPosition.position, Quaternion.identity);
    }

    private Vector3 GetCameraHalfSizeVector()
    {
        Vector3 cameraSize = new Vector3(mainCamera.orthographicSize * 1.3f, 0);
        return cameraSize;
    } 
}
