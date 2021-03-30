// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Player/PlayerInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputs"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f9f6c326-c2d8-48cc-9b8b-a25c1886e1cd"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""b8cfdf39-30e8-400a-b625-bfbd2832521a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Light"",
                    ""type"": ""Button"",
                    ""id"": ""e015b30f-fbd9-4010-812e-d11cf5c6a8b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""7e2d5d8c-93db-4ea3-9c59-0d45eb60a89b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""975de847-402d-4b37-864b-7134d012368a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Voice"",
                    ""type"": ""Button"",
                    ""id"": ""a8d9ed23-7c7c-488d-b82e-edc27884c801"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwitchWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""1fac1ea4-8686-4d99-a7c9-adfea3606962"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AltAttack"",
                    ""type"": ""Button"",
                    ""id"": ""4c839c3e-5dc2-472f-9ca5-c3fda2a31580"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HelpToggle"",
                    ""type"": ""Button"",
                    ""id"": ""eb7fc918-b88d-4cda-9fa1-dea493c92c45"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""94bf84f0-59e4-4fca-9f95-2a718df06eb9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""02578706-a338-4ade-aa91-80d3eeeeb50a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpectatorToggle"",
                    ""type"": ""Button"",
                    ""id"": ""5d5261e0-c0e9-4414-8f02-2e0fda1d3a2e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChangePersp"",
                    ""type"": ""Button"",
                    ""id"": ""c3b593d1-ae6e-495d-8a18-5add1634b221"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChangeRot"",
                    ""type"": ""Button"",
                    ""id"": ""a306812e-6984-478d-8b9c-43a2612eb5e8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b09d4084-ae41-4e10-8fc7-4d9287ce8b8f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""be1649fd-70a9-4e7b-b235-ec4041a0be50"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c72d36c1-da5f-4277-9761-c856fd30dff5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""69fc213c-92cc-432a-8ac2-3acb53fa3eca"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""be30e818-cc94-4ab8-a69f-6a434aacdbe6"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""10681b88-1a15-4cb4-97e8-60aa070eede4"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Light"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d0a34fec-86ed-434c-9c7a-dc247d955af8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1bb3fcc-8f51-48d0-99ee-d393cfc5f809"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d10967b1-722f-4544-a38c-ac0b5c1f6509"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Voice"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""816a5a60-d119-4d38-a5ca-d38ddff7d6b0"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""SwitchWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3371efcf-f30d-43b9-ac43-115ed909266f"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""AltAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""968e2c02-32d1-421f-868f-7fde85dbea0f"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""HelpToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fbbd26c9-ab4f-4e75-899c-2c1fb9371da0"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ac0e4261-340a-40c6-9a0e-8db262cc2812"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd5d27d2-5009-4a30-94e6-bb9655249956"",
                    ""path"": ""<Keyboard>/rightBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""SpectatorToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""02e80c23-39d4-463b-94b0-e5154070c40f"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""ChangePersp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a45bd7d7-2c06-4796-9814-ec60f527028b"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""ChangeRot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Light = m_Player.FindAction("Light", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
        m_Player_Voice = m_Player.FindAction("Voice", throwIfNotFound: true);
        m_Player_SwitchWeapon = m_Player.FindAction("SwitchWeapon", throwIfNotFound: true);
        m_Player_AltAttack = m_Player.FindAction("AltAttack", throwIfNotFound: true);
        m_Player_HelpToggle = m_Player.FindAction("HelpToggle", throwIfNotFound: true);
        m_Player_Pause = m_Player.FindAction("Pause", throwIfNotFound: true);
        m_Player_Reload = m_Player.FindAction("Reload", throwIfNotFound: true);
        m_Player_SpectatorToggle = m_Player.FindAction("SpectatorToggle", throwIfNotFound: true);
        m_Player_ChangePersp = m_Player.FindAction("ChangePersp", throwIfNotFound: true);
        m_Player_ChangeRot = m_Player.FindAction("ChangeRot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Light;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_Dash;
    private readonly InputAction m_Player_Voice;
    private readonly InputAction m_Player_SwitchWeapon;
    private readonly InputAction m_Player_AltAttack;
    private readonly InputAction m_Player_HelpToggle;
    private readonly InputAction m_Player_Pause;
    private readonly InputAction m_Player_Reload;
    private readonly InputAction m_Player_SpectatorToggle;
    private readonly InputAction m_Player_ChangePersp;
    private readonly InputAction m_Player_ChangeRot;
    public struct PlayerActions
    {
        private @PlayerInputs m_Wrapper;
        public PlayerActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Light => m_Wrapper.m_Player_Light;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @Dash => m_Wrapper.m_Player_Dash;
        public InputAction @Voice => m_Wrapper.m_Player_Voice;
        public InputAction @SwitchWeapon => m_Wrapper.m_Player_SwitchWeapon;
        public InputAction @AltAttack => m_Wrapper.m_Player_AltAttack;
        public InputAction @HelpToggle => m_Wrapper.m_Player_HelpToggle;
        public InputAction @Pause => m_Wrapper.m_Player_Pause;
        public InputAction @Reload => m_Wrapper.m_Player_Reload;
        public InputAction @SpectatorToggle => m_Wrapper.m_Player_SpectatorToggle;
        public InputAction @ChangePersp => m_Wrapper.m_Player_ChangePersp;
        public InputAction @ChangeRot => m_Wrapper.m_Player_ChangeRot;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Light.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLight;
                @Light.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLight;
                @Light.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLight;
                @Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Dash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Voice.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVoice;
                @Voice.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVoice;
                @Voice.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVoice;
                @SwitchWeapon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchWeapon;
                @SwitchWeapon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchWeapon;
                @SwitchWeapon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchWeapon;
                @AltAttack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAltAttack;
                @AltAttack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAltAttack;
                @AltAttack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAltAttack;
                @HelpToggle.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHelpToggle;
                @HelpToggle.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHelpToggle;
                @HelpToggle.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHelpToggle;
                @Pause.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Reload.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @SpectatorToggle.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpectatorToggle;
                @SpectatorToggle.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpectatorToggle;
                @SpectatorToggle.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpectatorToggle;
                @ChangePersp.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangePersp;
                @ChangePersp.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangePersp;
                @ChangePersp.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangePersp;
                @ChangeRot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeRot;
                @ChangeRot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeRot;
                @ChangeRot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeRot;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Light.started += instance.OnLight;
                @Light.performed += instance.OnLight;
                @Light.canceled += instance.OnLight;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Voice.started += instance.OnVoice;
                @Voice.performed += instance.OnVoice;
                @Voice.canceled += instance.OnVoice;
                @SwitchWeapon.started += instance.OnSwitchWeapon;
                @SwitchWeapon.performed += instance.OnSwitchWeapon;
                @SwitchWeapon.canceled += instance.OnSwitchWeapon;
                @AltAttack.started += instance.OnAltAttack;
                @AltAttack.performed += instance.OnAltAttack;
                @AltAttack.canceled += instance.OnAltAttack;
                @HelpToggle.started += instance.OnHelpToggle;
                @HelpToggle.performed += instance.OnHelpToggle;
                @HelpToggle.canceled += instance.OnHelpToggle;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @SpectatorToggle.started += instance.OnSpectatorToggle;
                @SpectatorToggle.performed += instance.OnSpectatorToggle;
                @SpectatorToggle.canceled += instance.OnSpectatorToggle;
                @ChangePersp.started += instance.OnChangePersp;
                @ChangePersp.performed += instance.OnChangePersp;
                @ChangePersp.canceled += instance.OnChangePersp;
                @ChangeRot.started += instance.OnChangeRot;
                @ChangeRot.performed += instance.OnChangeRot;
                @ChangeRot.canceled += instance.OnChangeRot;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLight(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnVoice(InputAction.CallbackContext context);
        void OnSwitchWeapon(InputAction.CallbackContext context);
        void OnAltAttack(InputAction.CallbackContext context);
        void OnHelpToggle(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnSpectatorToggle(InputAction.CallbackContext context);
        void OnChangePersp(InputAction.CallbackContext context);
        void OnChangeRot(InputAction.CallbackContext context);
    }
}
