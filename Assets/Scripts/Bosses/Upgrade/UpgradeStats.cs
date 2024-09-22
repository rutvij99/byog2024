using UnityEngine;

namespace Bosses.Upgrade
{
    public static class UpgradeStats
    {
        private const string KEY_PHY_LVL = "Phy_Level";
        private const string KEY_ELE_LVL = "Ele_Level";
        private const string KEY_POINTS = "Souls";
        private const string LIGHTENING_UNLOCKED = "lighening_unlocked";


        public static void UnlockLightening()
        {
            PlayerPrefs.SetInt(LIGHTENING_UNLOCKED, 1);
            PlayerPrefs.Save();
        }
        public static bool IsLighteningUnlocked()
        {
            return PlayerPrefs.HasKey(KEY_POINTS);
        }
        
        public static int GetSouls()
        {
            return PlayerPrefs.GetInt(KEY_POINTS, 0);
        }
        
        public static void AddSouls(int val)
        {
            PlayerPrefs.SetInt(KEY_POINTS, GetSouls() + val);
        }
        
        public static void Upgrade(UpgradeType type)
        {
            var key = type == UpgradeType.Physical ? KEY_PHY_LVL : KEY_ELE_LVL;
            int val = PlayerPrefs.GetInt(key, 1);
            PlayerPrefs.SetInt(key, val + 1);
            PlayerPrefs.Save();
        }

        public static int GetLevel(UpgradeType type)
        {
            var key = type == UpgradeType.Physical ? KEY_PHY_LVL : KEY_ELE_LVL;
            return PlayerPrefs.GetInt(key, 1);
        }

        public static float GetDamageMultiplier(UpgradeType type)
        {
            var so = Resources.Load<MultiplierLookUp>(type.ToString() +"_MultiplierLookupTable");
            int lvl = GetLevel(type);
            if (lvl >= so.Table.Count - 1)
                lvl = so.Table.Count - 1;
            return so.Table[lvl-1];
        }

        public static void FlushPrefs()
        {
            PlayerPrefs.DeleteKey(KEY_PHY_LVL);
            PlayerPrefs.DeleteKey(KEY_ELE_LVL);
            PlayerPrefs.DeleteKey(KEY_POINTS);
            PlayerPrefs.Save();
        }
    }
}