using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    [SerializeField]
    private NetworkManager networkManager;
    [SerializeField]
    private Button hostButton;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    //임시 공용 호스트 주소
    private const string HostAddress = "HostAddress";

    void Start()
    {
        if (networkManager == null)
        {
            networkManager = GetComponent<NetworkManager>();
        }

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steamworks not initialized");
            return;
        }

        hostButton.onClick.AddListener(Host);

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }
    /// <summary>
    /// 호스트 버튼 클릭시 스팀 로비 호스트
    /// </summary>
    public void Host()
    {
        hostButton.gameObject.SetActive(false);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }
    /// <summary>
    /// 로비 생성 콜백함수
    /// </summary>
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby");
            hostButton.gameObject.SetActive(true);
            return;
        }

        networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddress, SteamUser.GetSteamID().ToString());
    }
    // /// <summary>
    // /// 로비 참가 요청받을때 콜백함수
    // /// </summary>
    // private void OnLobbyJoinRequested(LobbyMatchList_t callback)
    // {
    //     if (callback.m_nLobbiesMatching == 0)
    //     {
    //         networkManager.StartClient();
    //         return;
    //     }

    //     for (int i = 0; i < callback.m_nLobbiesMatching; i++)
    //     {
    //         CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
    //         if (SteamMatchmaking.GetLobbyData(lobbyId, HostAddress) == SteamUser.GetSteamID().ToString())
    //         {
    //             SteamMatchmaking.JoinLobby(lobbyId);
    //             return;
    //         }
    //     }
    // }

    /// <summary>
    /// 로비 참가 요청 콜백함수
    /// </summary>
    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    /// <summary>
    /// 로비 참가 콜백함수
    /// </summary>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }
        hostButton.gameObject.SetActive(false);
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddress);
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }
}
