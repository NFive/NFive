using CitizenFX.Core;
using System.Collections.Generic;

namespace NFive.Client
{
	public static class KeyMapping
	{
		public static Dictionary<Control, JavaScriptCode> ControlMappings = new Dictionary<Control, JavaScriptCode>
		{
			// { Control.AccurateAim, JavaScriptCode.b_117 },
			// { Control.Aim, JavaScriptCode.b_101 },
			{ Control.Arrest, JavaScriptCode.KeyF },
			// { Control.Attack, JavaScriptCode.b_100 },
			// { Control.Attack2, JavaScriptCode.b_100 },
			{ Control.CharacterWheel, JavaScriptCode.AltRight },
			{ Control.CinematicSlowMo, JavaScriptCode.KeyL },
			{ Control.Context, JavaScriptCode.KeyE },
			{ Control.ContextSecondary, JavaScriptCode.KeyQ },
			{ Control.Cover, JavaScriptCode.KeyQ },
			{ Control.CreatorAccept, JavaScriptCode.NumpadEnter },
			{ Control.CreatorDelete, JavaScriptCode.Delete },
			{ Control.CreatorLS, JavaScriptCode.KeyR },
			{ Control.CreatorLT, JavaScriptCode.KeyX },
			{ Control.CreatorMenuToggle, JavaScriptCode.ShiftRight },
			{ Control.CreatorRS, JavaScriptCode.KeyF },
			{ Control.CreatorRT, JavaScriptCode.KeyC },
			// { Control.CursorAccept, JavaScriptCode.b_100 },
			// { Control.CursorCancel, JavaScriptCode.b_101 },
			// { Control.CursorScrollDown, JavaScriptCode.b_116 },
			// { Control.CursorScrollUp, JavaScriptCode.b_115 },
			// { Control.CursorX, JavaScriptCode.b_112 },
			// { Control.CursorY, JavaScriptCode.b_113 },
			{ Control.Detonate, JavaScriptCode.KeyG },
			{ Control.Dive, JavaScriptCode.Space },
			{ Control.DropAmmo, JavaScriptCode.F10 },
			{ Control.DropWeapon, JavaScriptCode.F9 },
			{ Control.Duck, JavaScriptCode.ControlLeft },
			{ Control.Enter, JavaScriptCode.KeyF },
			{ Control.EnterCheatCode, JavaScriptCode.Backquote },
			// { Control.FlyLeftRight, JavaScriptCode.t_D%t_A },
			// { Control.FlyUpDown, JavaScriptCode.t_S%t_W },
			{ Control.FrontendAccept, JavaScriptCode.NumpadEnter },
			// { Control.FrontendAxisX, JavaScriptCode.t_D%t_A },
			// { Control.FrontendAxisY, JavaScriptCode.t_S%t_W },
			{ Control.FrontendCancel, JavaScriptCode.Escape },
			{ Control.FrontendDelete, JavaScriptCode.Delete },
			{ Control.FrontendDown, JavaScriptCode.ArrowDown },
			{ Control.FrontendEndscreenAccept, JavaScriptCode.NumpadEnter },
			{ Control.FrontendEndscreenExpand, JavaScriptCode.Space },
			{ Control.FrontendLb, JavaScriptCode.KeyQ },
			{ Control.FrontendLeaderboard, JavaScriptCode.Tab },
			{ Control.FrontendLeft, JavaScriptCode.ArrowLeft },
			{ Control.FrontendLs, JavaScriptCode.ShiftRight },
			{ Control.FrontendLt, JavaScriptCode.PageDown },
			{ Control.FrontendPause, JavaScriptCode.KeyP },
			{ Control.FrontendPauseAlternate, JavaScriptCode.Escape },
			{ Control.FrontendRb, JavaScriptCode.KeyE },
			{ Control.FrontendRdown, JavaScriptCode.NumpadEnter },
			{ Control.FrontendRight, JavaScriptCode.ArrowRight },
			// { Control.FrontendRightAxisX, JavaScriptCode.t_]%t_[ },
			// { Control.FrontendRightAxisY, JavaScriptCode.b_117 },
			{ Control.FrontendRleft, JavaScriptCode.Space },
			{ Control.FrontendRright, JavaScriptCode.Backspace },
			{ Control.FrontendRs, JavaScriptCode.ControlLeft },
			{ Control.FrontendRt, JavaScriptCode.PageUp },
			{ Control.FrontendRup, JavaScriptCode.Tab },
			{ Control.FrontendSelect, JavaScriptCode.CapsLock },
			{ Control.FrontendSocialClub, JavaScriptCode.Home },
			{ Control.FrontendSocialClubSecondary, JavaScriptCode.Home },
			{ Control.FrontendUp, JavaScriptCode.ArrowUp },
			{ Control.FrontendX, JavaScriptCode.Space },
			{ Control.FrontendY, JavaScriptCode.Tab },
			{ Control.HUDSpecial, JavaScriptCode.KeyZ },
			{ Control.InteractionMenu, JavaScriptCode.KeyM },
			{ Control.Jump, JavaScriptCode.Space },
			{ Control.LookBehind, JavaScriptCode.KeyC },
			// { Control.LookDown, JavaScriptCode.b_995 },
			// { Control.LookDownOnly, JavaScriptCode.b_111 },
			// { Control.LookLeft, JavaScriptCode.b_995 },
			// { Control.LookLeftOnly, JavaScriptCode.b_108 },
			// { Control.LookLeftRight, JavaScriptCode.b_112 },
			// { Control.LookRight, JavaScriptCode.b_995 },
			// { Control.LookRightOnly, JavaScriptCode.b_109 },
			// { Control.LookUp, JavaScriptCode.b_995 },
			// { Control.LookUpDown, JavaScriptCode.b_113 },
			// { Control.LookUpOnly, JavaScriptCode.b_110 },
			// { Control.Map, JavaScriptCode.b_995 },
			// { Control.MapPointOfInterest, JavaScriptCode.b_102 },
			// { Control.MeleeAttack1, JavaScriptCode.b_995 },
			// { Control.MeleeAttack2, JavaScriptCode.b_995 },
			// { Control.MeleeAttackAlternate, JavaScriptCode.b_100 },
			{ Control.MeleeAttackHeavy, JavaScriptCode.KeyQ },
			{ Control.MeleeAttackLight, JavaScriptCode.KeyR },
			{ Control.MeleeBlock, JavaScriptCode.Space },
			// { Control.MoveDown, JavaScriptCode.b_995 },
			{ Control.MoveDownOnly, JavaScriptCode.KeyS },
			// { Control.MoveLeft, JavaScriptCode.b_995 },
			{ Control.MoveLeftOnly, JavaScriptCode.KeyA },
			// { Control.MoveLeftRight, JavaScriptCode.t_D%t_A },
			// { Control.MoveRight, JavaScriptCode.b_995 },
			{ Control.MoveRightOnly, JavaScriptCode.KeyD },
			// { Control.MoveUp, JavaScriptCode.b_995 },
			// { Control.MoveUpDown, JavaScriptCode.t_S%t_W },
			{ Control.MoveUpOnly, JavaScriptCode.KeyW },
			{ Control.MpTextChatAll, JavaScriptCode.KeyT },
			// { Control.MpTextChatCrew, JavaScriptCode.b_995 },
			// { Control.MpTextChatFriends, JavaScriptCode.b_995 },
			{ Control.MpTextChatTeam, JavaScriptCode.KeyY },
			{ Control.MultiplayerInfo, JavaScriptCode.KeyZ },
			{ Control.NextCamera, JavaScriptCode.KeyV },
			// { Control.NextWeapon, JavaScriptCode.b_995 },
			{ Control.ParachuteBrakeLeft, JavaScriptCode.KeyQ },
			{ Control.ParachuteBrakeRight, JavaScriptCode.KeyE },
			// { Control.ParachuteDeploy, JavaScriptCode.b_100 },
			{ Control.ParachuteDetach, JavaScriptCode.KeyF },
			{ Control.ParachutePitchDownOnly, JavaScriptCode.KeyS },
			// { Control.ParachutePitchUpDown, JavaScriptCode.t_S%t_W },
			{ Control.ParachutePitchUpOnly, JavaScriptCode.KeyW },
			{ Control.ParachutePrecisionLanding, JavaScriptCode.ShiftRight },
			{ Control.ParachuteSmoke, JavaScriptCode.KeyX },
			{ Control.ParachuteTurnLeftOnly, JavaScriptCode.KeyA },
			// { Control.ParachuteTurnLeftRight, JavaScriptCode.t_D%t_A },
			{ Control.ParachuteTurnRightOnly, JavaScriptCode.KeyD },
			{ Control.Phone, JavaScriptCode.ArrowUp },
			{ Control.PhoneCameraDOF, JavaScriptCode.KeyF },
			{ Control.PhoneCameraExpression, JavaScriptCode.KeyX },
			{ Control.PhoneCameraFocusLock, JavaScriptCode.KeyL },
			{ Control.PhoneCameraGrid, JavaScriptCode.KeyG },
			// { Control.PhoneCameraSelfie, JavaScriptCode.b_102 },
			{ Control.PhoneCancel, JavaScriptCode.Backspace },
			{ Control.PhoneDown, JavaScriptCode.ArrowDown },
			{ Control.PhoneExtraOption, JavaScriptCode.Space },
			{ Control.PhoneLeft, JavaScriptCode.ArrowLeft },
			{ Control.PhoneOption, JavaScriptCode.Delete },
			{ Control.PhoneRight, JavaScriptCode.ArrowRight },
			// { Control.PhoneScrollBackward, JavaScriptCode.b_115 },
			// { Control.PhoneScrollForward, JavaScriptCode.b_116 },
			{ Control.PhoneSelect, JavaScriptCode.NumpadEnter },
			{ Control.PhoneUp, JavaScriptCode.ArrowUp },
			{ Control.Pickup, JavaScriptCode.KeyE },
			// { Control.PrevWeapon, JavaScriptCode.b_995 },
			{ Control.PushToTalk, JavaScriptCode.KeyN },
			// { Control.RadioWheelLeftRight, JavaScriptCode.b_112 },
			// { Control.RadioWheelUpDown, JavaScriptCode.b_113 },
			// { Control.RappelJump, JavaScriptCode.b_995 },
			// { Control.RappelLongJump, JavaScriptCode.b_995 },
			// { Control.RappelSmashWindow, JavaScriptCode.b_995 },
			{ Control.Reload, JavaScriptCode.KeyR },
			{ Control.ReplayAdvance, JavaScriptCode.ArrowRight },
			{ Control.ReplayBack, JavaScriptCode.ArrowLeft },
			{ Control.ReplayCameraDown, JavaScriptCode.PageDown },
			{ Control.ReplayCameraUp, JavaScriptCode.PageUp },
			{ Control.ReplayClipDelete, JavaScriptCode.Delete },
			{ Control.ReplayCtrl, JavaScriptCode.ControlLeft },
			{ Control.ReplayCycleMarkerLeft, JavaScriptCode.BracketLeft },
			{ Control.ReplayCycleMarkerRight, JavaScriptCode.BracketRight },
			{ Control.ReplayEndpoint, JavaScriptCode.KeyN },
			{ Control.ReplayFfwd, JavaScriptCode.ArrowUp },
			{ Control.ReplayFOVDecrease, JavaScriptCode.NumpadSubtract },
			{ Control.ReplayFOVIncrease, JavaScriptCode.NumpadAdd },
			{ Control.ReplayHidehud, JavaScriptCode.KeyH },
			{ Control.ReplayMarkerDelete, JavaScriptCode.Delete },
			{ Control.ReplayNewmarker, JavaScriptCode.KeyM },
			{ Control.ReplayPause, JavaScriptCode.Space },
			{ Control.ReplayPreview, JavaScriptCode.Space },
			{ Control.ReplayPreviewAudio, JavaScriptCode.Space },
			{ Control.ReplayRecord, JavaScriptCode.KeyS },
			{ Control.ReplayRestart, JavaScriptCode.KeyR },
			{ Control.ReplayRewind, JavaScriptCode.ArrowDown },
			{ Control.ReplaySave, JavaScriptCode.F5 },
			{ Control.ReplayScreenshot, JavaScriptCode.KeyU },
			{ Control.ReplayShowhotkey, JavaScriptCode.KeyK },
			{ Control.ReplaySnapmaticPhoto, JavaScriptCode.Tab },
			{ Control.ReplayStartpoint, JavaScriptCode.KeyB },
			{ Control.ReplayStartStopRecording, JavaScriptCode.F1 },
			{ Control.ReplayStartStopRecordingSecondary, JavaScriptCode.F2 },
			{ Control.ReplayTimelineDuplicateClip, JavaScriptCode.KeyC },
			{ Control.ReplayTimelinePickupClip, JavaScriptCode.KeyX },
			{ Control.ReplayTimelinePlaceClip, JavaScriptCode.KeyV },
			{ Control.ReplayTimelineSave, JavaScriptCode.F5 },
			{ Control.ReplayToggletime, JavaScriptCode.KeyC },
			{ Control.ReplayToggleTimeline, JavaScriptCode.Escape },
			{ Control.ReplayToggletips, JavaScriptCode.KeyV },
			{ Control.ReplayTools, JavaScriptCode.KeyT },
			{ Control.SaveReplayClip, JavaScriptCode.F3 },
			// { Control.ScaledLookDownOnly, JavaScriptCode.b_111 },
			// { Control.ScaledLookLeftOnly, JavaScriptCode.b_108 },
			// { Control.ScaledLookLeftRight, JavaScriptCode.b_112 },
			// { Control.ScaledLookRightOnly, JavaScriptCode.b_109 },
			// { Control.ScaledLookUpDown, JavaScriptCode.b_113 },
			// { Control.ScaledLookUpOnly, JavaScriptCode.b_110 },
			{ Control.ScriptedFlyZDown, JavaScriptCode.PageDown },
			{ Control.ScriptedFlyZUp, JavaScriptCode.PageUp },
			// { Control.ScriptLB, JavaScriptCode.b_995 },
			// { Control.ScriptLeftAxisX, JavaScriptCode.t_D%t_A },
			// { Control.ScriptLeftAxisY, JavaScriptCode.t_S%t_W },
			// { Control.ScriptLS, JavaScriptCode.b_995 },
			// { Control.ScriptLT, JavaScriptCode.b_995 },
			{ Control.ScriptPadDown, JavaScriptCode.KeyS },
			{ Control.ScriptPadLeft, JavaScriptCode.KeyA },
			{ Control.ScriptPadRight, JavaScriptCode.KeyD },
			{ Control.ScriptPadUp, JavaScriptCode.KeyW },
			// { Control.ScriptRB, JavaScriptCode.b_995 },
			// { Control.ScriptRDown, JavaScriptCode.b_100 },
			// { Control.ScriptRightAxisX, JavaScriptCode.b_112 },
			// { Control.ScriptRightAxisY, JavaScriptCode.b_113 },
			{ Control.ScriptRLeft, JavaScriptCode.ControlLeft },
			// { Control.ScriptRRight, JavaScriptCode.b_101 },
			// { Control.ScriptRS, JavaScriptCode.b_995 },
			// { Control.ScriptRT, JavaScriptCode.b_100 },
			// { Control.ScriptRUp, JavaScriptCode.b_101 },
			{ Control.ScriptSelect, JavaScriptCode.KeyV },
			{ Control.SelectCharacterFranklin, JavaScriptCode.F6 },
			{ Control.SelectCharacterMichael, JavaScriptCode.F5 },
			{ Control.SelectCharacterMultiplayer, JavaScriptCode.F8 },
			{ Control.SelectCharacterTrevor, JavaScriptCode.F7 },
			// { Control.SelectNextWeapon, JavaScriptCode.b_116 },
			// { Control.SelectPrevWeapon, JavaScriptCode.b_115 },
			{ Control.SelectWeapon, JavaScriptCode.Tab },
			{ Control.SelectWeaponAutoRifle, JavaScriptCode.Digit8 },
			{ Control.SelectWeaponHandgun, JavaScriptCode.Digit6 },
			{ Control.SelectWeaponHeavy, JavaScriptCode.Digit4 },
			{ Control.SelectWeaponMelee, JavaScriptCode.Digit2 },
			{ Control.SelectWeaponShotgun, JavaScriptCode.Digit3 },
			{ Control.SelectWeaponSmg, JavaScriptCode.Digit7 },
			{ Control.SelectWeaponSniper, JavaScriptCode.Digit9 },
			{ Control.SelectWeaponSpecial, JavaScriptCode.Digit5 },
			{ Control.SelectWeaponUnarmed, JavaScriptCode.Digit1 },
			// { Control.SkipCutscene, JavaScriptCode.b_100 },
			// { Control.SniperZoom, JavaScriptCode.b_117 },
			// { Control.SniperZoomIn, JavaScriptCode.b_995 },
			// { Control.SniperZoomInAlternate, JavaScriptCode.b_995 },
			// { Control.SniperZoomInOnly, JavaScriptCode.b_115 },
			// { Control.SniperZoomInSecondary, JavaScriptCode.b_115 },
			// { Control.SniperZoomOut, JavaScriptCode.b_995 },
			// { Control.SniperZoomOutAlternate, JavaScriptCode.b_995 },
			// { Control.SniperZoomOutOnly, JavaScriptCode.b_116 },
			// { Control.SniperZoomOutSecondary, JavaScriptCode.b_116 },
			// { Control.SpecialAbility, JavaScriptCode.b_995 },
			{ Control.SpecialAbilityPC, JavaScriptCode.CapsLock },
			{ Control.SpecialAbilitySecondary, JavaScriptCode.KeyB },
			{ Control.Sprint, JavaScriptCode.ShiftRight },
			{ Control.SwitchVisor, JavaScriptCode.F11 },
			{ Control.Talk, JavaScriptCode.KeyE },
			{ Control.ThrowGrenade, JavaScriptCode.KeyG },
			{ Control.VehicleAccelerate, JavaScriptCode.KeyW },
			// { Control.VehicleAim, JavaScriptCode.b_101 },
			// { Control.VehicleAttack, JavaScriptCode.b_100 },
			{ Control.VehicleAttack2, JavaScriptCode.ControlLeft },
			{ Control.VehicleBikeWings, JavaScriptCode.KeyX },
			{ Control.VehicleBrake, JavaScriptCode.KeyS },
			{ Control.VehicleCarJump, JavaScriptCode.KeyE },
			{ Control.VehicleCinCam, JavaScriptCode.KeyR },
			// { Control.VehicleCinematicDownOnly, JavaScriptCode.b_116 },
			// { Control.VehicleCinematicLeftRight, JavaScriptCode.b_112 },
			// { Control.VehicleCinematicUpDown, JavaScriptCode.b_113 },
			// { Control.VehicleCinematicUpOnly, JavaScriptCode.b_115 },
			// { Control.VehicleDriveLook, JavaScriptCode.b_100 },
			// { Control.VehicleDriveLook2, JavaScriptCode.b_101 },
			{ Control.VehicleDropProjectile, JavaScriptCode.KeyX },
			{ Control.VehicleDuck, JavaScriptCode.KeyX },
			{ Control.VehicleExit, JavaScriptCode.KeyF },
			{ Control.VehicleFlyAttack, JavaScriptCode.Space },
			{ Control.VehicleFlyAttack2, JavaScriptCode.Space },
			{ Control.VehicleFlyAttackCamera, JavaScriptCode.Insert },
			{ Control.VehicleFlyBombBay, JavaScriptCode.KeyE },
			{ Control.VehicleFlyBoost, JavaScriptCode.ShiftRight },
			{ Control.VehicleFlyCounter, JavaScriptCode.KeyE },
			{ Control.VehicleFlyDuck, JavaScriptCode.KeyX },
			// { Control.VehicleFlyMouseControlOverride, JavaScriptCode.b_100 },
			{ Control.VehicleFlyPitchDownOnly, JavaScriptCode.Numpad5 },
			// { Control.VehicleFlyPitchUpDown, JavaScriptCode.b_141%b_144 },
			{ Control.VehicleFlyPitchUpOnly, JavaScriptCode.Numpad8 },
			{ Control.VehicleFlyRollLeftOnly, JavaScriptCode.Numpad4 },
			// { Control.VehicleFlyRollLeftRight, JavaScriptCode.b_142%b_140 },
			{ Control.VehicleFlyRollRightOnly, JavaScriptCode.Numpad6 },
			// { Control.VehicleFlySelectNextWeapon, JavaScriptCode.b_115 },
			{ Control.VehicleFlySelectPrevWeapon, JavaScriptCode.BracketLeft },
			{ Control.VehicleFlySelectTargetLeft, JavaScriptCode.Numpad7 },
			{ Control.VehicleFlySelectTargetRight, JavaScriptCode.Numpad9 },
			{ Control.VehicleFlyThrottleDown, JavaScriptCode.KeyS },
			{ Control.VehicleFlyThrottleUp, JavaScriptCode.KeyW },
			{ Control.VehicleFlyTransform, JavaScriptCode.KeyX },
			{ Control.VehicleFlyUnderCarriage, JavaScriptCode.KeyG },
			{ Control.VehicleFlyVerticalFlightMode, JavaScriptCode.KeyE },
			{ Control.VehicleFlyYawLeft, JavaScriptCode.KeyA },
			{ Control.VehicleFlyYawRight, JavaScriptCode.KeyD },
			{ Control.VehicleGrapplingHook, JavaScriptCode.KeyE },
			// { Control.VehicleGunDown, JavaScriptCode.b_995 },
			// { Control.VehicleGunLeft, JavaScriptCode.b_995 },
			// { Control.VehicleGunLeftRight, JavaScriptCode.b_112 },
			// { Control.VehicleGunRight, JavaScriptCode.b_995 },
			// { Control.VehicleGunUp, JavaScriptCode.b_995 },
			// { Control.VehicleGunUpDown, JavaScriptCode.b_113 },
			{ Control.VehicleHandbrake, JavaScriptCode.Space },
			{ Control.VehicleHeadlight, JavaScriptCode.KeyH },
			{ Control.VehicleHorn, JavaScriptCode.KeyE },
			{ Control.VehicleHotwireLeft, JavaScriptCode.KeyW },
			{ Control.VehicleHotwireRight, JavaScriptCode.KeyS },
			{ Control.VehicleHydraulicsControlDown, JavaScriptCode.ControlLeft },
			{ Control.VehicleHydraulicsControlLeft, JavaScriptCode.KeyA },
			// { Control.VehicleHydraulicsControlLeftRight, JavaScriptCode.b_1013%b_1000 },
			{ Control.VehicleHydraulicsControlRight, JavaScriptCode.KeyD },
			{ Control.VehicleHydraulicsControlToggle, JavaScriptCode.KeyX },
			{ Control.VehicleHydraulicsControlUp, JavaScriptCode.ShiftRight },
			// { Control.VehicleHydraulicsControlUpDown, JavaScriptCode.t_D%t_A },
			{ Control.VehicleJump, JavaScriptCode.Space },
			{ Control.VehicleLookBehind, JavaScriptCode.KeyC },
			// { Control.VehicleLookLeft, JavaScriptCode.b_995 },
			// { Control.VehicleLookRight, JavaScriptCode.b_995 },
			{ Control.VehicleMeleeHold, JavaScriptCode.KeyX },
			// { Control.VehicleMeleeLeft, JavaScriptCode.b_100 },
			// { Control.VehicleMeleeRight, JavaScriptCode.b_101 },
			// { Control.VehicleMouseControlOverride, JavaScriptCode.b_100 },
			// { Control.VehicleMoveDown, JavaScriptCode.b_995 },
			{ Control.VehicleMoveDownOnly, JavaScriptCode.ControlLeft },
			// { Control.VehicleMoveLeft, JavaScriptCode.b_995 },
			{ Control.VehicleMoveLeftOnly, JavaScriptCode.KeyA },
			// { Control.VehicleMoveLeftRight, JavaScriptCode.t_D%t_A },
			// { Control.VehicleMoveRight, JavaScriptCode.b_995 },
			{ Control.VehicleMoveRightOnly, JavaScriptCode.KeyD },
			// { Control.VehicleMoveUp, JavaScriptCode.b_995 },
			// { Control.VehicleMoveUpDown, JavaScriptCode.b_1013%b_1000 },
			{ Control.VehicleMoveUpOnly, JavaScriptCode.ShiftRight },
			{ Control.VehicleNextRadio, JavaScriptCode.Period },
			{ Control.VehicleNextRadioTrack, JavaScriptCode.Equal },
			{ Control.VehicleParachute, JavaScriptCode.Space },
			// { Control.VehiclePassengerAim, JavaScriptCode.b_101 },
			// { Control.VehiclePassengerAttack, JavaScriptCode.b_100 },
			{ Control.VehiclePrevRadio, JavaScriptCode.Comma },
			{ Control.VehiclePrevRadioTrack, JavaScriptCode.Minus },
			{ Control.VehiclePushbikeFrontBrake, JavaScriptCode.KeyQ },
			{ Control.VehiclePushbikePedal, JavaScriptCode.KeyW },
			{ Control.VehiclePushbikeRearBrake, JavaScriptCode.KeyS },
			{ Control.VehiclePushbikeSprint, JavaScriptCode.CapsLock },
			{ Control.VehicleRadioWheel, JavaScriptCode.KeyQ },
			{ Control.VehicleRocketBoost, JavaScriptCode.KeyE },
			{ Control.VehicleRoof, JavaScriptCode.KeyH },
			// { Control.VehicleSelectNextWeapon, JavaScriptCode.b_115 },
			{ Control.VehicleSelectPrevWeapon, JavaScriptCode.BracketLeft },
			{ Control.VehicleShuffle, JavaScriptCode.KeyH },
			// { Control.VehicleSlowMoDownOnly, JavaScriptCode.b_116 },
			// { Control.VehicleSlowMoUpDown, JavaScriptCode.b_117 },
			// { Control.VehicleSlowMoUpOnly, JavaScriptCode.b_115 },
			// { Control.VehicleSpecial, JavaScriptCode.b_995 },
			// { Control.VehicleSpecialAbilityFranklin, JavaScriptCode.b_995 },
			// { Control.VehicleStuntUpDown, JavaScriptCode.b_995 },
			{ Control.VehicleSubAscend, JavaScriptCode.ShiftRight },
			{ Control.VehicleSubDescend, JavaScriptCode.ControlLeft },
			// { Control.VehicleSubMouseControlOverride, JavaScriptCode.b_100 },
			{ Control.VehicleSubPitchDownOnly, JavaScriptCode.Numpad5 },
			// { Control.VehicleSubPitchUpDown, JavaScriptCode.b_141%b_144 },
			{ Control.VehicleSubPitchUpOnly, JavaScriptCode.Numpad8 },
			{ Control.VehicleSubThrottleDown, JavaScriptCode.KeyS },
			{ Control.VehicleSubThrottleUp, JavaScriptCode.KeyW },
			{ Control.VehicleSubTurnHardLeft, JavaScriptCode.KeyA },
			{ Control.VehicleSubTurnHardRight, JavaScriptCode.KeyD },
			{ Control.VehicleSubTurnLeftOnly, JavaScriptCode.Numpad4 },
			// { Control.VehicleSubTurnLeftRight, JavaScriptCode.b_142%b_140 },
			{ Control.VehicleSubTurnRightOnly, JavaScriptCode.Numpad6 },
			// { Control.WeaponSpecial, JavaScriptCode.b_995 },
			{ Control.WeaponSpecial2, JavaScriptCode.KeyE },
			// { Control.WeaponWheelLeftRight, JavaScriptCode.b_-1 },
			// { Control.WeaponWheelNext, JavaScriptCode.b_116 },
			// { Control.WeaponWheelPrev, JavaScriptCode.b_115 },
			// { Control.WeaponWheelUpDown, JavaScriptCode.b_-1 },
			// { Control.Whistle, JavaScriptCode.b_995 },
		};

		public static Dictionary<string, JavaScriptCode> KeyMappings = new Dictionary<string, JavaScriptCode>
		{
			{ "b_-1", JavaScriptCode.None }, // Correct?
			{ "b_995", JavaScriptCode.None }, // Correct?

			//{ "b_100", Key.LeftMouseClick },
			//{ "b_101", Key.RightMouseClick },
			//{ "b_102", Key.MiddleMouseClick },

			//{ "b_108", Key.MouseMoveLeft },
			//{ "b_109", Key.MouseMoveRight },
			//{ "b_110", Key.MouseMoveUp },
			//{ "b_111", Key.MouseMoveDown },
			//{ "b_112", Key.MouseMoveLeftRight },
			//{ "b_113", Key.MouseMoveUpDown },

			//{ "b_115", Key.MouseWheelUp },
			//{ "b_116", Key.MouseWheelDown },
			//{ "b_117", Key.MouseWheelUpDown },

			{ "b_130", JavaScriptCode.NumpadSubtract },
			{ "b_131", JavaScriptCode.NumpadAdd },
			{ "b_132", JavaScriptCode.NumpadDecimal },
			{ "b_133", JavaScriptCode.NumpadDivide },
			{ "b_134", JavaScriptCode.NumpadMultiply },
			{ "b_135", JavaScriptCode.NumpadEnter },
			{ "b_136", JavaScriptCode.Numpad0 },
			{ "b_137", JavaScriptCode.Numpad1 },
			{ "b_138", JavaScriptCode.Numpad2 },
			{ "b_139", JavaScriptCode.Numpad3 },
			{ "b_140", JavaScriptCode.Numpad4 },
			{ "b_141", JavaScriptCode.Numpad5 },
			{ "b_142", JavaScriptCode.Numpad6 },
			{ "b_143", JavaScriptCode.Numpad7 },
			{ "b_144", JavaScriptCode.Numpad8 },
			{ "b_145", JavaScriptCode.Numpad9 },
			{ "b_146", JavaScriptCode.NumpadEqual },

			{ "b_170", JavaScriptCode.F1 },
			{ "b_171", JavaScriptCode.F2 },
			{ "b_172", JavaScriptCode.F3 },
			{ "b_173", JavaScriptCode.F4 },
			{ "b_174", JavaScriptCode.F5 },
			{ "b_175", JavaScriptCode.F6 },
			{ "b_176", JavaScriptCode.F7 },
			{ "b_177", JavaScriptCode.F8 },
			{ "b_178", JavaScriptCode.F9 },
			{ "b_179", JavaScriptCode.F10 },
			{ "b_180", JavaScriptCode.F11 },
			{ "b_181", JavaScriptCode.F12 },
			{ "b_182", JavaScriptCode.F13 },
			{ "b_183", JavaScriptCode.F14 },
			{ "b_184", JavaScriptCode.F15 },
			{ "b_185", JavaScriptCode.F16 },
			{ "b_186", JavaScriptCode.F17 },
			{ "b_187", JavaScriptCode.F18 },
			{ "b_188", JavaScriptCode.F19 },
			{ "b_189", JavaScriptCode.F20 },
			{ "b_190", JavaScriptCode.F21 },
			{ "b_191", JavaScriptCode.F22 },
			{ "b_192", JavaScriptCode.F23 },
			{ "b_193", JavaScriptCode.F24 },
			{ "b_194", JavaScriptCode.ArrowUp },
			{ "b_195", JavaScriptCode.ArrowDown },
			{ "b_196", JavaScriptCode.ArrowLeft },
			{ "b_197", JavaScriptCode.ArrowRight },
			{ "b_198", JavaScriptCode.Delete },
			{ "b_199", JavaScriptCode.Escape },
			{ "b_200", JavaScriptCode.Insert },
			{ "b_201", JavaScriptCode.End },

			{ "b_1000", JavaScriptCode.ShiftLeft },
			{ "b_1001", JavaScriptCode.ShiftRight },
			{ "b_1002", JavaScriptCode.Tab },
			{ "b_1003", JavaScriptCode.Return },
			{ "b_1004", JavaScriptCode.Backspace },
			{ "b_1005", JavaScriptCode.PrintScreen },
			{ "b_1006", JavaScriptCode.ScrollLock },
			{ "b_1007", JavaScriptCode.Pause },
			{ "b_1008", JavaScriptCode.Home },
			{ "b_1009", JavaScriptCode.PageUp },
			{ "b_1010", JavaScriptCode.PageDown },
			{ "b_1011", JavaScriptCode.NumLock },
			{ "b_1012", JavaScriptCode.CapsLock },
			{ "b_1013", JavaScriptCode.ControlLeft },
			{ "b_1014", JavaScriptCode.ControlRight },
			{ "b_1015", JavaScriptCode.AltLeft },
			{ "b_1016", JavaScriptCode.AltRight },
			{ "b_1017", JavaScriptCode.ContextMenu },
			{ "b_1018", JavaScriptCode.MetaLeft },
			{ "b_1019", JavaScriptCode.MetaRight },

			{ "b_2000", JavaScriptCode.Space },

			{ "t_+", JavaScriptCode.Equal },
			{ "t_-", JavaScriptCode.Minus },
			{ "t_,", JavaScriptCode.Comma },
			{ "t_;", JavaScriptCode.Semicolon },
			{ "t_.", JavaScriptCode.Period },
			{ "t_[", JavaScriptCode.BracketLeft },
			{ "t_]", JavaScriptCode.BracketRight },
			{ "t_`", JavaScriptCode.Backquote },
			{ "t_=", JavaScriptCode.Equal },
			{ "t_0", JavaScriptCode.Digit0 },
			{ "t_1", JavaScriptCode.Digit1 },
			{ "t_2", JavaScriptCode.Digit2 },
			{ "t_3", JavaScriptCode.Digit3 },
			{ "t_4", JavaScriptCode.Digit4 },
			{ "t_5", JavaScriptCode.Digit5 },
			{ "t_6", JavaScriptCode.Digit6 },
			{ "t_7", JavaScriptCode.Digit7 },
			{ "t_8", JavaScriptCode.Digit8 },
			{ "t_9", JavaScriptCode.Digit9 },
			{ "t_A", JavaScriptCode.KeyA },
			{ "t_B", JavaScriptCode.KeyB },
			{ "t_C", JavaScriptCode.KeyC },
			{ "t_D", JavaScriptCode.KeyD },
			{ "t_E", JavaScriptCode.KeyE },
			{ "t_F", JavaScriptCode.KeyF },
			{ "t_G", JavaScriptCode.KeyG },
			{ "t_H", JavaScriptCode.KeyH },
			{ "t_I", JavaScriptCode.KeyI },
			{ "t_J", JavaScriptCode.KeyJ },
			{ "t_K", JavaScriptCode.KeyK },
			{ "t_L", JavaScriptCode.KeyL },
			{ "t_M", JavaScriptCode.KeyM },
			{ "t_N", JavaScriptCode.KeyN },
			{ "t_O", JavaScriptCode.KeyO },
			{ "t_P", JavaScriptCode.KeyP },
			{ "t_Q", JavaScriptCode.KeyQ },
			{ "t_R", JavaScriptCode.KeyR },
			{ "t_S", JavaScriptCode.KeyS },
			{ "t_T", JavaScriptCode.KeyT },
			{ "t_U", JavaScriptCode.KeyU },
			{ "t_V", JavaScriptCode.KeyV },
			{ "t_W", JavaScriptCode.KeyW },
			{ "t_X", JavaScriptCode.KeyX },
			{ "t_Y", JavaScriptCode.KeyY },
			{ "t_Z", JavaScriptCode.KeyZ }
		};
	}
}
