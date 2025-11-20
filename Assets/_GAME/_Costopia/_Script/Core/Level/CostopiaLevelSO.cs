using System.Threading.Tasks;


using UnityEngine;

namespace Costopia.Level
{
  [CreateAssetMenu(fileName = "CostopiaLv", menuName = "Costopia/CostopiaLevel", order = 0)]
  public class CostopiaLevelSO : ScriptableObject
  {
    #region Customer Type

    public enum CustomerType
    {
      Human = 0,
      Animal = 1,
      Item = 2,
    }

    private const string DIALOGUE_SHORT_KEY = "_dialogue_intro_short";
    private const string DIALOGUE_LONG_KEY = "_dialogue_intro_long";
    private const string DIALOGUE_THANKS_KEY = "_dialogue_thanks";
    private const string DIALOGUE_STATUS_KEY = "_dialogue_end_caption";
    private const string LEVEL_PREFIX = "lv_";


    #endregion

    #region Attribute


    [SerializeField]
    private string nameId;

    [SerializeField]
    private int levelId;

    [SerializeField]
    private CustomerType customerType;

    [SerializeField]
    private Sprite avatar;

    public string NameId => nameId;
    public int LevelId => levelId;
    public CustomerType Type => customerType;
    public Sprite Avatar => avatar;

    #endregion

    #region Scene, Thumb, Hint
    public static string LevelIdToSceneName(int levelId) => "Level" + levelId;
    public static string LevelIdToThumbnailName(int levelId) => "thumb-costopia-lv-" + levelId;
    public static string LevelIdToHintName(int levelId) => "hint-level-" + levelId;


    public string SceneName => LevelIdToSceneName(levelId);

    public string ThumbnailName => LevelIdToThumbnailName(levelId);

    #endregion
  }
}