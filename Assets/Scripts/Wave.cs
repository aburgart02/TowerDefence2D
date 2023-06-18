using UnityEngine;

namespace DefaultNamespace
{
    [System.Serializable]
    public class Wave
    {
        [SerializeField]
        public int Light;
        [SerializeField]
        public int Medium;
        [SerializeField]
        public int Heavy;

        public Wave(int light, int medium, int heavy)
        {
            Light = light;
            Medium = medium;
            Heavy = heavy;
        }

        public int TotalCount => Light + Medium + Heavy;
    }
}