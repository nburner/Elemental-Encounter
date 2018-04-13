using UnityEngine;
using UnityEngine.UI;

namespace NetworkGame
{
    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class RoomNameInputField : MonoBehaviour
    {
        public InputField input;

        #region Private Variables

        // Store the PlayerPref Key to avoid typos
        static string playerNamePrefKey = "Player";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            string defaultName = "";
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                input.text = defaultName;
            }

            PhotonNetwork.playerName = defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName()
        {
            // #Important
            PhotonNetwork.playerName = input.text + " "; // force a trailing space string in case value is an empty string, else playerName would not be updated.            
            PlayerPrefs.SetString(playerNamePrefKey, input.text);
            PlayerPrefs.Save();
        }

        #endregion
    }
}