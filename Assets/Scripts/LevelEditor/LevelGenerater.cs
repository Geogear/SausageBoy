using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerater : MonoBehaviour
{
    [SerializeField] List<GameObject> levels = null;
    [SerializeField] public Transform levelEndPosition; 
    [SerializeField] Player player = null;
    [SerializeField] Camera mainCamera = null;
    private int currentLevelIndex;
    private int prevLevelIndex;
    public float levelWidth = 23.58f;

    // Start is called before the first frame update
    void Start()
    {
        currentLevelIndex = 0;
        prevLevelIndex = 0;
        SpawnLevel(levels[0], mainCamera.transform);
    }

    // Update is called once per frame
    void Update()
    {
        GenerateLevel();
        DeletePreviousLevel();
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
            SpawnLevel(levels[currentLevelIndex], levelEndPosition);  
            levelEndPosition.position = new Vector3(levelEndPosition.position.x + levelWidth, levelEndPosition.position.y);
        }
    }

    private bool LevelCanBeGenerated()
    {
        if (player.transform.position.x  >= levelEndPosition.position.x)
        {
            return true;
        }
        return false;
    }

    private void SpawnLevel(GameObject level, Transform levelPosition)
    {
        Vector2 newPos = new Vector2(levelPosition.position.x, levelPosition.position.y);
        newPos += (Vector2)GetCameraHalfSizeVector();
        Instantiate(level, newPos, Quaternion.identity);
    }

    private Vector3 GetCameraHalfSizeVector()
    {
        Vector3 cameraSize = new Vector3(mainCamera.orthographicSize * 1.3f, 0);
        return cameraSize;
    }  

    private void DeletePreviousLevel()
    {
        /*Vector3 leftSide = new Vector3(mainCamera.transform.position.x - mainCamera.orthographicSize, 0);
        if ( leftSide.x - levels[prevLevelIndex].GetComponent<LevelEditor>().levelEndPosition.position.x >= 0)
        {
            DestroyImmediate(levels[prevLevelIndex]);
        }*/
    }
}
