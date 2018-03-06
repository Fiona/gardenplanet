using System.Security.Permissions;
using UnityEngine;

namespace StrawberryNova
{
    public class GameStateFile
    {
        public Character.Appearence playerAppearence;
        public Character.Information playerInformation;
    }

    /*
     * This is the global game state and is responsible for saving and loading.
     */
    public class GameState : MonoBehaviour
    {
        private static GameState instance = null;

        private Character.Appearence playerAppearence;
        private Character.Information playerInformation;

        public static GameState GetInstance()
        {
            if(instance != null)
                return instance;
            instance = new GameObject("GameState").AddComponent<GameState>();
            DontDestroyOnLoad(instance.gameObject);
            instance.Clear();
            return instance;
        }

        public void Clear()
        {
            playerAppearence = Player.defaultAppearence;
            playerInformation = Player.defaultInformation;
        }

        public void InitialiseGame(Player player)
        {
            player.SetAppearence(playerAppearence);
            player.SetInformation(playerInformation);
        }

        /*
         * STORE METHOD: Player objects
         */
        public void Store(Player player)
        {
            Store((Character)player);
        }

        /*
         * STORE METHOD: Character objects
         */
        public void Store(Character player)
        {
            playerAppearence = player.GetAppearence();
            playerInformation = player.GetInformation();
        }
    }
}