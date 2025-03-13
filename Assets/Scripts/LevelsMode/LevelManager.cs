using UnityEngine;

namespace WordBoggle
{
    /// <summary>
    /// Responsible for loading level data, can be extended to create and save further levels
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        private LevelModeData _levelData;
        private void Start()
        {
            TextAsset levelDataJson = Resources.Load<TextAsset>("levelData");
            if (levelDataJson != null)
            {
                Debug.Log(levelDataJson);
                _levelData = JsonUtility.FromJson<LevelModeData>(levelDataJson.text);
            }
            else
            {
                Debug.LogWarning("Level Data file is missing.");
            }
            
        }
        
        public LevelData LoadLevel(int levelNumber)
        {
            if(levelNumber <= 0) levelNumber = 0;
            if (levelNumber >= _levelData.data.Count)
            {
                Debug.LogWarning("Level number is out of range.");
                return null;
            }
            return _levelData.data[levelNumber];
        }
        
    }
}
