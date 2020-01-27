// Copyright (c) Csaba Molnar & Daniel Butum. All Rights Reserved.

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tools.DotNETCommon;

using UnrealBuildTool;


public class SOrb : ModuleRules
{
	// PublicDefinition flags
	//
	// NOTE: These flags have the default values for the desktop (usually)
	//

	// Enables/disable GameAnalytics plugin
	// NOTE: UWAJIMAYA_WITH_ANALYTICS needs be also true
	public bool UWAJIMAYA_WITH_GAMEANALYTICS = false;

	// Enables/disable the analytics providers, all of THEM
	public bool UWAJIMAYA_WITH_ANALYTICS = false;

	// Enable/disable the default collection of analytics
	public readonly bool UWAJIMAYA_COLLECT_ANALYTICS = true;

	// off by default
	public bool UWAJIMAYA_WITH_ONLINE = false;

	// Enables/disable SDL2 plugin
	public readonly bool UWAJIMAYA_WITH_SDL2 = false;

	// Enable/disable the char fake shadow
	public readonly bool UWAJIMAYA_USE_CHARACTER_SHADOW = true;

	// Enable/disable the usage of the unreal save system
	// If this is enabled we use the unreal save (ISaveGameSystem) for saving and loading. These files are binary
	// NOTE: usually on consoles as our own implementation of the save does not support those platforms
	public readonly bool UWAJIMAYA_USE_UNREAL_SAVE_SYSTEM = false;

	// Special variable we can override to fake non editor behaviour in editor
	// Usually just equal with WITH_EDITOR
	public readonly bool UWAJIMAYA_WITH_EDITOR = false;

	//
	// INPUT
	//

	// Should we use the keyboard arrow preset as the default?
	public readonly bool UWAJIMAYA_USE_ARROW_PRESET_AS_DEFAULT = true;

	//
	// DEBUG
	//

	// Simulate non editor behaviour
	// Set it to true to have non-editor behavior in editor regarding game start, saves, etc.
	// Mostly used in places where !WITH_EDITOR would be used
	// NOTE: does not affect analytics, use UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST for that
	public const bool UWAJIMAYA_NON_EDITOR_TEST = false;

	// Simulate the collecting of events analytics like in the non editor
	public const bool UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST = false;

	// Simulate as we are on a console
	public const bool UWAJIMAYA_CONSOLE_TEST = false;

	// Simulate like the platform does not support keyboard
	public const bool UWAJIMAYA_NO_KEYBOARD_TEST = false;

	public SOrb(ReadOnlyTargetRules Target) : base(Target)
	{
		Console.WriteLine("Using SOrb.Build.cs");
		Init();

		if (!IsSupportedConsolePlatform && !IsSupportedDesktopPlatform)
		{
			Fail("Target platform is not supported");
		}

		// Aka WITH_EDITOR && !UWAJIMAYA_NON_EDITOR_TEST
		UWAJIMAYA_WITH_EDITOR = Target.bBuildEditor && !UWAJIMAYA_NON_EDITOR_TEST;

		// Do not collect analytics in editor, only if the debug flag is set to true
		// Aka !WITH_EDITOR || UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST
		UWAJIMAYA_COLLECT_ANALYTICS = !Target.bBuildEditor || UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST;

		// Console flags take precedence
		if (IsPlatformSwitch)
		{
			if (UWAJIMAYA_USE_CHARACTER_SHADOW)
			{
				PrintYellow("Disabling the Character fake shadow (UWAJIMAYA_USE_CHARACTER_SHADOW)");
				UWAJIMAYA_USE_CHARACTER_SHADOW = false;
			}
		}
		if (IsSupportedConsolePlatform)
		{
			PrintYellow("Disabling Analytics & GameAnalytics support because we are building for console");
			UWAJIMAYA_WITH_ANALYTICS = false;
			UWAJIMAYA_COLLECT_ANALYTICS = false;

			if (UWAJIMAYA_WITH_SDL2)
			{
				PrintYellow("Disabling SDL2 support because we are building for console");
				UWAJIMAYA_WITH_SDL2 = false;
			}
			if (UWAJIMAYA_WITH_DISCORD)
			{
				PrintYellow("Disabling Discord support because we are building for console");
				UWAJIMAYA_WITH_DISCORD = false;
			}
			if (UWAJIMAYA_WITH_STEAM)
			{
				PrintYellow("Disabling Steam support because we are building for console");
				UWAJIMAYA_WITH_STEAM = false;
			}

			PrintYellow("Enabling (UWAJIMAYA_USE_UNREAL_SAVE_SYSTEM) the the Unreal Save System (ISaveGameSystem)");
			UWAJIMAYA_USE_UNREAL_SAVE_SYSTEM = true;
		}
		if (Target.bBuildEditor)
		{
			if (UWAJIMAYA_NON_EDITOR_TEST)
			{
				PrintYellow("[DEBUG] UWAJIMAYA_NON_EDITOR_TEST is set to TRUE, UWAJIMAYA_WITH_EDITOR will be set to FALSE");
			}
			if (UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST)
			{
				PrintYellow("[DEBUG] UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST is set to TRUE, UWAJIMAYA_COLLECT_ANALYTICS will be set to TRUE");
			}

			if (UWAJIMAYA_WITH_DISCORD)
			{
				PrintYellow("Disabling UWAJIMAYA_WITH_DISCORD because we are building for Editor");
				UWAJIMAYA_WITH_DISCORD = false;
			}
			if (UWAJIMAYA_WITH_STEAM)
			{
				PrintYellow("Disabling UWAJIMAYA_WITH_STEAM because we are building for Editor");
				UWAJIMAYA_WITH_STEAM = false;
			}
		}

		if (IsPlatformXboxOne)
		{
			PrintYellow("Enabling online because XBOX");
			UWAJIMAYA_WITH_ONLINE = true;
		}

		if (UWAJIMAYA_DEMO)
		{
			PrintBlue("[DEMO] BUILD [DEMO]");
			PrintYellow("[DEMO] BUILD [DEMO]");
			PrintRed("[DEMO] BUILD [DEMO]");
			Console.WriteLine("");

			if (UWAJIMAYA_WITH_VIDEO_INTRO)
			{
				PrintYellow("Disabling UWAJIMAYA_WITH_VIDEO_INTRO because we are building for Demo");
				UWAJIMAYA_WITH_VIDEO_INTRO = false;
			}
		}
		if (!UWAJIMAYA_WITH_ANALYTICS)
		{
			UWAJIMAYA_WITH_GAMEANALYTICS = false;
			PrintYellow("Disabling GameAnalytics (UWAJIMAYA_WITH_ANALYTICS) because UWAJIMAYA_WITH_ANALYTICS is false");
		}
		if (UWAJIMAYA_WITH_STEAM)
		{
			PrintYellow("Enabling online because we have steam enabled");
			UWAJIMAYA_WITH_ONLINE = true;

			// Disable some flags that should only be used in a NON demo build
			if (UWAJIMAYA_DEMO || Target.bBuildEditor)
			{
				if (UWAJIMAYA_RELAUNCH_IN_STEAM)
				{
					PrintYellow("Disabling UWAJIMAYA_RELAUNCH_IN_STEAM because we are building for Demo/Editor");
					UWAJIMAYA_RELAUNCH_IN_STEAM = false;
				}

				if (UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES)
				{
					PrintYellow("Disabling UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES because we are building for Demo/Editor");
					UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES = false;
				}
			}

			// Disable some flags that should only be used in a Windows64 build
			if (!IsPlatformWindows64)
			{
				if (UWAJIMAYA_RELAUNCH_IN_STEAM)
				{
					PrintYellow("Disabling UWAJIMAYA_RELAUNCH_IN_STEAM because we are NOT building for Windows64");
					UWAJIMAYA_RELAUNCH_IN_STEAM = false;
				}

				if (UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES)
				{
					PrintYellow("Disabling UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES because we are NOT building for Windows64");
					UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES = false;
				}
			}
		}
		else
		{
			if (UWAJIMAYA_RELAUNCH_IN_STEAM)
			{
				PrintYellow("Disabling UWAJIMAYA_RELAUNCH_IN_STEAM because we are NOT building for Steam");
				UWAJIMAYA_RELAUNCH_IN_STEAM = false;
			}
			if (UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES)
			{
				PrintYellow("Disabling UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES because we are NOT building for Steam");
				UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES = false;
			}
		}

		//
		// Flags to use in C++
		//

		// DEBUG FLAGS
		PublicDefinitions.Add("UWAJIMAYA_NON_EDITOR_TEST=" + BoolToIntString(UWAJIMAYA_NON_EDITOR_TEST));
		PublicDefinitions.Add("UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST=" + BoolToIntString(UWAJIMAYA_NON_EDITOR_COLLECT_ANALYTICS_TEST));
		PublicDefinitions.Add("UWAJIMAYA_WITH_EDITOR=" + BoolToIntString(UWAJIMAYA_WITH_EDITOR));
		PublicDefinitions.Add("UWAJIMAYA_CONSOLE_TEST=" + BoolToIntString(UWAJIMAYA_CONSOLE_TEST));
		PublicDefinitions.Add("UWAJIMAYA_NO_KEYBOARD_TEST=" + BoolToIntString(UWAJIMAYA_NO_KEYBOARD_TEST));

		// Plugins
		PublicDefinitions.Add("UWAJIMAYA_WITH_ONLINE=" + BoolToIntString(UWAJIMAYA_WITH_ONLINE));

		// Enable video demo support
		PublicDefinitions.Add("UWAJIMAYA_WITH_VIDEO_DEMO=" + BoolToIntString(UWAJIMAYA_WITH_VIDEO_DEMO));

		PublicDefinitions.Add("UWAJIMAYA_WITH_VIDEO_INTRO=" + BoolToIntString(UWAJIMAYA_WITH_VIDEO_INTRO));

		// Enable demo support aka Uwajimaya: Prologue
		PublicDefinitions.Add("UWAJIMAYA_DEMO=" + BoolToIntString(UWAJIMAYA_DEMO));

		// Game flags
		PublicDefinitions.Add("UWAJIMAYA_USE_CHARACTER_SHADOW=" + BoolToIntString(UWAJIMAYA_USE_CHARACTER_SHADOW));
		PublicDefinitions.Add("UWAJIMAYA_USE_UNREAL_SAVE_SYSTEM=" + BoolToIntString(UWAJIMAYA_USE_UNREAL_SAVE_SYSTEM));
		PublicDefinitions.Add("UWAJIMAYA_USE_ARROW_PRESET_AS_DEFAULT=" + BoolToIntString(UWAJIMAYA_USE_ARROW_PRESET_AS_DEFAULT));
		PublicDefinitions.Add("UWAJIMAYA_ALLOW_CONSOLE_CHEATS_DEFAULT=" + BoolToIntString(UWAJIMAYA_ALLOW_CONSOLE_CHEATS_DEFAULT));
		PublicDefinitions.Add(String.Format("UWAJIMAYA_PASSWORD_ENABLE_CONSOLE_CHEATS=\"{0}\"", UWAJIMAYA_PASSWORD_ENABLE_CONSOLE_CHEATS));
		PublicDefinitions.Add(String.Format("UWAJIMAYA_PASSWORD_SAVE_FILE=\"{0}\"", UWAJIMAYA_PASSWORD_SAVE_FILE));

		if (UWAJIMAYA_WITH_ONLINE)
		{
			PublicDependencyModuleNames.Add("OnlineSubsystem");
			PublicDependencyModuleNames.Add("OnlineSubsystemUtils");
			if (IsPlatformXboxOne)
				PublicDependencyModuleNames.Add("OnlineSubsystemLive");
			else
				PublicDependencyModuleNames.Add("OnlineSubsystemNull");
			//PublicDependencyModuleNames.Add("OnlineFramework");
		}

		SetupBuildVersion();
		SetupSteam();
		SetupLoadingScreen();
		SetupSDL();
		SetupAsusAuraSDK();
		SetupDiscord();
		SetupAnalytics();

		// We need this dependency to get some key information from viewport when moving object along spline
		if (Target.bBuildEditor == true)
		{
			PrivateDependencyModuleNames.Add("UnrealEd");
		}

		PublicIncludePaths.AddRange(new string[] {
			Path.Combine(ModuleDirectory)
		});

		// Print Normal flags
		Console.WriteLine("PublicDefinitions Flags:");
		foreach (string Defintion in PublicDefinitions)
		{
			PrintBlue(Defintion);
		}
		Console.WriteLine("");

		//Console.WriteLine("ProjectDefinitions Flags:");
		//foreach (string Defintion in Target.ProjectDefinitions)
		//{
		//	PrintBlue(Defintion);
		//}
		//Console.WriteLine("");

		// Now get the base of UE4's modules dir (could also be Developer, Editor, ThirdParty)
		//string source_runtime_path = Path.Combine(EngineRoot, "Source", "Runtime");
		//if (Target.Platform == UnrealTargetPlatform.Win64 || Target.Platform == UnrealTargetPlatform.Win32)
		//{
		//	PublicIncludePaths.Add(Path.Combine(source_runtime_path, "ApplicationCore", "Private"));
		//}

		// Debug info disable
		// PrintBlue(string.Format("ENGINE_VERSION_MAJOR={0}", Target.Version.MajorVersion));
		// PrintBlue(string.Format("ENGINE_VERSION_MINOR={0}", Target.Version.MinorVersion));
		// PrintBlue(string.Format("ENGINE_VERSION_HOTFIX={0}", Target.Version.PatchVersion));
		// PrintBlue(string.Format("ENGINE_IS_LICENSEE_VERSION={0}", Target.Version.IsLicenseeVersion? "true" : "false"));
		// PrintBlue(string.Format("ENGINE_IS_PROMOTED_BUILD={0}", Target.Version.IsPromotedBuild? "true" : "false"));
		// PrintBlue(string.Format("CURRENT_CHANGELIST={0}", Target.Version.Changelist));
		// PrintBlue(string.Format("COMPATIBLE_CHANGELIST={0}", Target.Version.EffectiveCompatibleChangelist));
		// PrintBlue(string.Format("BRANCH_NAME=\"{0}\"", Target.Version.BranchName));
		// PrintBlue(string.Format("BUILD_VERSION=\"{0}\"", Target.BuildVersion));
	}

	public void Init()
	{
		InitFromPlatform(Target.Platform);
		ParseBuildFile();

		//
		// Common
		//

		// Enable IWYU
		// https://docs.unrealengine.com/latest/INT/Programming/UnrealBuildSystem/IWYUReferenceGuide/index.html
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
		bEnforceIWYU = true;
		//PrivatePCHHeaderFile = "SOrb.h";

		// Faster compile time optimization
		// MinFilesUsingPrecompiledHeaderOverride = 1;
		// If this is enabled, it makes each file compile by itself, default UE is to bundle files together into Modules
		bFasterWithoutUnity = false;

		// Other, note older CPUS than 2012 don't work
		bUseAVX = false;

		PublicDependencyModuleNames.AddRange(new string[] {
			"Core",
			"ApplicationCore",
			"CoreUObject",
			"Engine",
			"InputCore",
			"AIModule",
			"Json",
			"AnimationCore",
			"AnimGraphRuntime",
			"UMG",
			"Slate",
			"SlateCore",
			"ApexDestruction",

			"MovieScene",
			"LevelSequence",
			"MediaAssets",
			"Media",
			"AudioMixer",

			"RHI", // getting system stuff

			// Init stuff
			"SoBeforeGame"
		});

		PrivateDependencyModuleNames.AddRange(new string[] {
			// Plugins
			 "DlgSystem",
			 "FMODStudio",
			 "NotYetLoadingScreen"
		});
	}

	public string CurrentDirectoryPath
	{
		get
		{
			return Path.GetFullPath(ModuleDirectory);
		}
	}
	public string UwajimayaVersionFilePath
	{
		get
		{
			return Path.Combine(CurrentDirectoryPath, "Resources", "UwajimayaVersion.h");
		}
	}

	public string EngineRootPath
	{
		// Get the engine path. Ends with "Engine/"
		get
		{
			return Path.GetFullPath(Target.RelativeEnginePath);
		}
	}

	public void SetupSteam()
	{
		// C++ Flags
		PublicDefinitions.Add("UWAJIMAYA_WITH_STEAM=" + BoolToIntString(UWAJIMAYA_WITH_STEAM));
		PublicDefinitions.Add("UWAJIMAYA_STEAM_APP_ID=" + IntToString(UWAJIMAYA_STEAM_APP_ID));
		PublicDefinitions.Add("UWAJIMAYA_STEAM_DEMO_APP_ID=" + IntToString(UWAJIMAYA_STEAM_DEMO_APP_ID));
		PublicDefinitions.Add("UWAJIMAYA_RELAUNCH_IN_STEAM=" + BoolToIntString(UWAJIMAYA_RELAUNCH_IN_STEAM));
		PublicDefinitions.Add("UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES=" + BoolToIntString(UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES));
		PublicDefinitions.Add(String.Format("UWAJIMAYA_STEAM_LIBRARY_WINDOWS_API64_SHA1_SUM=\"{0}\"", UWAJIMAYA_STEAM_LIBRARY_WINDOWS_API64_SHA1_SUM));

		ConfigHierarchy ConfigEngineHierarchy = ConfigCache.ReadHierarchy(ConfigHierarchyType.Engine, new DirectoryReference(ProjectRootPath), Target.Platform);
		Dictionary<string, string> SteamReplacements = new Dictionary<string, string>();

		// Steam
		if (UWAJIMAYA_WITH_STEAM)
		{
			// Replace the steam id with our own defined one
			// Same for the Game Version

			// SteamDevAppId
			{
				int CurrentAppid;
				ConfigEngineHierarchy.GetInt32("OnlineSubsystemSteam", "SteamDevAppId", out CurrentAppid);
				string FormatString = "SteamDevAppId={0}";
				SteamReplacements.Add(String.Format(FormatString, CurrentAppid), String.Format(FormatString, SteamAppid));
			}

			// GameVersion
			{
				string CurrentGameVersion;
				ConfigEngineHierarchy.GetString("OnlineSubsystemSteam", "GameVersion", out CurrentGameVersion);
				string FormatString = "GameVersion={0}";
				SteamReplacements.Add(String.Format(FormatString, CurrentGameVersion), String.Format(FormatString, UWAJIMAYA_BUILD_VERSION));
			}

			// Relaunch in Steam?
			// https://partner.steamgames.com/doc/api/steam_api#SteamAPI_RestartAppIfNecessary
			// NOTE: disabled at it is broken anyways
			// {
			// 	bool CurrentRelaunchInSteam;
			// 	ConfigEngineHierarchy.GetBool("OnlineSubsystemSteam", "bRelaunchInSteam", out CurrentRelaunchInSteam);
			// 	string FormatString = "bRelaunchInSteam={0}";
			// 	SteamReplacements.Add(String.Format(FormatString, CurrentRelaunchInSteam), String.Format(FormatString, UWAJIMAYA_RELAUNCH_IN_STEAM));
			// }

			// Include plugins
			PrivateDependencyModuleNames.Add("NotYetSteam");
			DynamicallyLoadedModuleNames.Add("OnlineSubsystemSteam");
		}

		// bEnabled
		// {
		// 	bool CurrentEnabled;
		// 	ConfigEngineHierarchy.GetBool("OnlineSubsystemSteam", "bEnabled", out CurrentEnabled);
		// 	string FormatString = "bEnabled={0}";
		// 	SteamReplacements.Add(String.Format(FormatString, CurrentEnabled), String.Format(FormatString, UWAJIMAYA_WITH_STEAM));
		// }

		PatchFile(DefaultEngineConfigPath, SteamReplacements);
	}

	public void SetupLoadingScreen()
	{
		// Loading screen replacements
		Dictionary<string, string> LoadingReplacements = new Dictionary<string, string>();
		string Format = "(Image={0},";
		string LoadingImageFinal = String.Format(Format, LoadingImage);
		string LoadingImageDemoFinal = String.Format(Format, LoadingImageDemo);
		if (UWAJIMAYA_DEMO)
		{
			// Replace main game image with demo
			LoadingReplacements.Add(LoadingImageFinal, LoadingImageDemoFinal);
		}
		else
		{
			// Replace demo image with main game
			LoadingReplacements.Add(LoadingImageDemoFinal, LoadingImageFinal);
		}
		PatchFile(DefaultNYLoadingScreenSettingsConfigPath, LoadingReplacements);
	}

	public void SetupSDL()
	{
		// C++ Flags
		PublicDefinitions.Add("UWAJIMAYA_WITH_SDL2=" + BoolToIntString(UWAJIMAYA_WITH_SDL2));
		if (UWAJIMAYA_WITH_SDL2)
		{
			PrivateDependencyModuleNames.Add("NotYetSDL2");
		}

		// For USoPlatformeHelper
		// if (IsPlatformLinux)
		// {
		// 	AddEngineThirdPartyPrivateStaticDependencies(Target, "SDL2");
		// }
	}

	public void SetupAsusAuraSDK()
	{
		PublicDefinitions.Add("UWAJIMAYA_USE_ASUS_AURA_SDK=" + BoolToIntString(UWAJIMAYA_USE_ASUS_AURA_SDK));
	}

	public void SetupDiscord()
	{
		PublicDefinitions.Add("UWAJIMAYA_WITH_DISCORD=" + BoolToIntString(UWAJIMAYA_WITH_DISCORD));
		PublicDefinitions.Add("UWAJIMAYA_DISCORD_CLIENT_ID=" + Int64ToString(UWAJIMAYA_DISCORD_CLIENT_ID));
		PublicDefinitions.Add("UWAJIMAYA_DISCORD_DEMO_CLIENT_ID=" + Int64ToString(UWAJIMAYA_DISCORD_DEMO_CLIENT_ID));

		if (UWAJIMAYA_WITH_DISCORD)
		{
			PrivateDependencyModuleNames.Add("DiscordGameSDK");
			// PrivateDependencyModuleNames.Add("DiscordGameSDKLibrary");
			PrivateIncludePathModuleNames.Add("DiscordGameSDK");
		}
	}

	public void SetupAnalytics()
	{
		// C++ Flags
		// Enable/disable analytics entirely or only the GameAnalytics plugin
		PublicDefinitions.Add("UWAJIMAYA_WITH_GAMEANALYTICS=" + BoolToIntString(UWAJIMAYA_WITH_GAMEANALYTICS));
		PublicDefinitions.Add("UWAJIMAYA_WITH_ANALYTICS=" + BoolToIntString(UWAJIMAYA_WITH_ANALYTICS));
		PublicDefinitions.Add("UWAJIMAYA_COLLECT_ANALYTICS=" + BoolToIntString(UWAJIMAYA_COLLECT_ANALYTICS));

		if (UWAJIMAYA_WITH_ANALYTICS)
		{
			PrivateDependencyModuleNames.Add("Analytics");
			PrivateIncludePathModuleNames.Add("Analytics");

			if (UWAJIMAYA_WITH_GAMEANALYTICS)
			{
				PrivateDependencyModuleNames.Add("GameAnalytics");
				PrivateIncludePathModuleNames.Add("GameAnalytics");
			}
		}
	}

	public void SetupBuildVersion()
	{
		string BUILD_VERSION = UWAJIMAYA_BUILD_VERSION;
		string BRANCH = "None";
		string COMMIT = "None";
		string DEMO = UWAJIMAYA_DEMO ? "-DEMO" : "";

		// Read build version
		GetCurrentBranchAndCommit(ref BRANCH, ref COMMIT);

		Console.WriteLine("");
		PrintBlue(String.Format("BUILD_VERSION = {0}, BRANCH = {1}, COMMIT = {2}, DEMO = {3}", BUILD_VERSION, BRANCH, COMMIT, UWAJIMAYA_DEMO));
		Console.WriteLine("");

		// The current build number
		// NOTE: this also sets the GameAnalytics version for all platforms, so you do not need to update those values.
		PublicDefinitions.Add(String.Format("UWAJIMAYA_BUILD_VERSION=\"{0}\"", BUILD_VERSION));
		PublicDefinitions.Add(String.Format("UWAJIMAYA_BUILD_BRANCH=\"{0}\"", BRANCH));
		PublicDefinitions.Add(String.Format("UWAJIMAYA_BUILD_COMMIT=\"{0}\"", COMMIT));

		// Write to file so that other resources can use it
		string VersionFileText = @"
// DO NOT MODIFY - FILE AUTOMATICALLY GENERATED in SOrb.Build.cs - DO NOT MODIFY
#define UWAJIMAYA_BUILD_VERSION {0}
#define UWAJIMAYA_BUILD_BRANCH {1}
#define UWAJIMAYA_BUILD_COMMIT {2}
#define UWAJIMAYA_BUILD_ALL {0}-{1}-{2}{3}
// DO NOT MODIFY - FILE AUTOMATICALLY GENERATED in SOrb.Build.cs - DO NOT MODIFY
";
		string VersionFileTextFormat = String.Format(VersionFileText, BUILD_VERSION, BRANCH, COMMIT, DEMO);
		File.WriteAllText(UwajimayaVersionFilePath, VersionFileTextFormat);

		// Do replacements in GeneralProjectSettings
		ConfigHierarchy ConfigGameHierarchy = ConfigCache.ReadHierarchy(ConfigHierarchyType.Game, new DirectoryReference(ProjectRootPath), Target.Platform);
		Dictionary<string, string> GeneralProjectSettingsReplacements = new Dictionary<string, string>();

		// ProjectVersion
		{
			string CurrentProjectVersion;
			ConfigGameHierarchy.GetString("/Script/EngineSettings.GeneralProjectSettings", "ProjectVersion", out CurrentProjectVersion);
			string FormatString = "ProjectVersion={0}";
			GeneralProjectSettingsReplacements.Add(String.Format(FormatString, CurrentProjectVersion), String.Format(FormatString, UWAJIMAYA_BUILD_VERSION));
		}

		// ProjectName
		{
			string CurrentProjectName;
			ConfigGameHierarchy.GetString("/Script/EngineSettings.GeneralProjectSettings", "ProjectName", out CurrentProjectName);
			string FormatString = "ProjectName={0}";
			string ReplacementString = UWAJIMAYA_DEMO ? ProjectNameDemo : ProjectName;
			GeneralProjectSettingsReplacements.Add(String.Format(FormatString, CurrentProjectName), String.Format(FormatString, ReplacementString));
		}

		// ProjectDisplayedTitle
		{
			string CurrentProjectDisplayedTitle;
			ConfigGameHierarchy.GetString("/Script/EngineSettings.GeneralProjectSettings", "ProjectDisplayedTitle", out CurrentProjectDisplayedTitle);
			string FormatString = "ProjectDisplayedTitle={0}";
			string ReplacementString = UWAJIMAYA_DEMO ? ProjectDisplayedTitleDemo : ProjectDisplayedTitle;
			GeneralProjectSettingsReplacements.Add(String.Format(FormatString, CurrentProjectDisplayedTitle), String.Format(FormatString, ReplacementString));
		}

		PatchFile(DefaultGameConfigPath, GeneralProjectSettingsReplacements);
	}

	//
	// BEGIN COMMON
	//

	//
	// Platform
	//

	// Constant variables because C# is stupid
	public bool IsSupportedDesktopPlatform = false;
	public bool IsSupportedConsolePlatform = false;

	public bool IsPlatformWindows64 = false;
	public bool IsPlatformLinux = false;
	public bool IsPlatformSwitch = false;
	public bool IsPlatformXboxOne = false;

	public void InitFromPlatform(UnrealTargetPlatform TargetPlatform)
	{
		// Checks if we can have some flags
		IsPlatformWindows64 = TargetPlatform == UnrealTargetPlatform.Win64;
		IsPlatformLinux = TargetPlatform == UnrealTargetPlatform.Linux;
		IsSupportedDesktopPlatform = IsPlatformWindows64 || IsPlatformLinux;

		IsPlatformSwitch = TargetPlatform == UnrealTargetPlatform.Switch;
		IsPlatformXboxOne = TargetPlatform == UnrealTargetPlatform.XboxOne;
		IsSupportedConsolePlatform = IsPlatformXboxOne || IsPlatformSwitch;
	}

	//
	// From Build file
	//

	public const string UWAJIMAYA_BUILD_FILE_NAME = "UwajimayaBuild.json";
	public string ProjectRootPath { get { return Target.ProjectFile.Directory.ToString(); } }
	public string ProjectSourceDirPath { get { return ProjectRootPath + "/Source"; } }

	public string ProjectConfigDirPath { get { return ProjectRootPath + "/Config"; } }
	public string DefaultEngineConfigPath { get { return ProjectConfigDirPath + "/DefaultEngine.ini"; } }
	public string DefaultGameConfigPath { get { return ProjectConfigDirPath + "/DefaultGame.ini"; } }
	public string DefaultNYLoadingScreenSettingsConfigPath { get { return ProjectConfigDirPath + "/DefaultNYLoadingScreenSettings.ini"; } }

	public string BuildConfigFilePath { get { return ProjectSourceDirPath + "/" + UWAJIMAYA_BUILD_FILE_NAME; } }


	//
	// Steam
	//

	public bool UWAJIMAYA_WITH_STEAM = false;
	public int UWAJIMAYA_STEAM_APP_ID = 0;
	public int UWAJIMAYA_STEAM_DEMO_APP_ID = 0;

	// https://partner.steamgames.com/doc/api/steam_api#SteamAPI_RestartAppIfNecessary
	public bool UWAJIMAYA_RELAUNCH_IN_STEAM = false;

	public bool UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES = false;

	public string UWAJIMAYA_STEAM_LIBRARY_WINDOWS_API64_SHA1_SUM = "";

	//
	// Discord
	//

	public bool UWAJIMAYA_WITH_DISCORD = false;
	public long UWAJIMAYA_DISCORD_CLIENT_ID = 0;
	public long UWAJIMAYA_DISCORD_DEMO_CLIENT_ID = 0;



	//
	// Video
	//

	// Enable/disable the video demo (for example at dreamhack/devplay)
	// Next time maybe look at: GIsDemoMode
	public bool UWAJIMAYA_WITH_VIDEO_DEMO = false;
	public bool UWAJIMAYA_WITH_VIDEO_INTRO = false;

	//
	// Other
	//

	public bool UWAJIMAYA_DEMO = false;

	public bool UWAJIMAYA_USE_ASUS_AURA_SDK = false;

	public string UWAJIMAYA_BUILD_VERSION;

	public bool UWAJIMAYA_ALLOW_CONSOLE_CHEATS_DEFAULT = false;

	public string UWAJIMAYA_PASSWORD_ENABLE_CONSOLE_CHEATS;

	public string UWAJIMAYA_PASSWORD_SAVE_FILE;

	public string LoadingImage;
	public string LoadingImageDemo;

	public string ProjectName;
	public string ProjectNameDemo;
	public string ProjectDisplayedTitle;
	public string ProjectDisplayedTitleDemo;


	public int SteamAppid { get { return UWAJIMAYA_DEMO ? UWAJIMAYA_STEAM_DEMO_APP_ID : UWAJIMAYA_STEAM_APP_ID; } }

	public void ParseBuildFile()
	{
		PrintBlue(String.Format("Reading from BuildConfigFile = {0}", BuildConfigFilePath));
		var AllFields = JsonObject.Read(new FileReference(BuildConfigFilePath));
		var PublicDefinitionsFields = AllFields.GetObjectField("PublicDefinitions");
		var LoadingFields = AllFields.GetObjectField("Loading");
		var SteamFields = AllFields.GetObjectField("Steam");
		var DiscordFields = AllFields.GetObjectField("Discord");
		var VideoFields = AllFields.GetObjectField("Video");


		ProjectName = AllFields.GetStringField("Name");
		ProjectNameDemo = AllFields.GetStringField("NameDemo");
		ProjectDisplayedTitle = AllFields.GetStringField("DisplayedTitle");
		ProjectDisplayedTitleDemo = AllFields.GetStringField("DisplayedTitleDemo");

		UWAJIMAYA_BUILD_VERSION = AllFields.GetStringField("Version");
		UWAJIMAYA_DEMO = PublicDefinitionsFields.GetBoolField("UWAJIMAYA_DEMO");
		UWAJIMAYA_USE_ASUS_AURA_SDK = PublicDefinitionsFields.GetBoolField("UWAJIMAYA_USE_ASUS_AURA_SDK");

		UWAJIMAYA_WITH_STEAM = SteamFields.GetBoolField("IS_ENABLED");
		UWAJIMAYA_STEAM_APP_ID = SteamFields.GetIntegerField("APP_ID");
		UWAJIMAYA_STEAM_DEMO_APP_ID = SteamFields.GetIntegerField("APP_ID_DEMO");
		UWAJIMAYA_RELAUNCH_IN_STEAM = SteamFields.GetBoolField("RELAUNCH_IN_STEAM");
		UWAJIMAYA_STEAM_CHECK_IF_LIBRARY_CHECKSUM_MATCHES = SteamFields.GetBoolField("CHECK_IF_LIBRARY_CHECKSUM_MATCHES");
		UWAJIMAYA_STEAM_LIBRARY_WINDOWS_API64_SHA1_SUM = SteamFields.GetStringField("LIBRARY_WINDOWS_API64_SHA1_SUM");

		UWAJIMAYA_WITH_DISCORD = DiscordFields.GetBoolField("IS_ENABLED");
		UWAJIMAYA_DISCORD_CLIENT_ID = Convert.ToInt64(DiscordFields.GetStringField("CLIENT_ID"));
		UWAJIMAYA_DISCORD_DEMO_CLIENT_ID = Convert.ToInt64(DiscordFields.GetStringField("CLIENT_ID_DEMO"));

		UWAJIMAYA_WITH_VIDEO_DEMO = VideoFields.GetBoolField("DEMO");
		UWAJIMAYA_WITH_VIDEO_INTRO = VideoFields.GetBoolField("INTRO");

		UWAJIMAYA_ALLOW_CONSOLE_CHEATS_DEFAULT = PublicDefinitionsFields.GetBoolField("UWAJIMAYA_ALLOW_CONSOLE_CHEATS_DEFAULT");
		UWAJIMAYA_PASSWORD_ENABLE_CONSOLE_CHEATS = PublicDefinitionsFields.GetStringField("UWAJIMAYA_PASSWORD_ENABLE_CONSOLE_CHEATS");
		UWAJIMAYA_PASSWORD_SAVE_FILE = PublicDefinitionsFields.GetStringField("UWAJIMAYA_PASSWORD_SAVE_FILE");

		LoadingImage = LoadingFields.GetStringField("Image");
		LoadingImageDemo = LoadingFields.GetStringField("ImageDemo");
	}

	//
	// Other
	//

	static void PatchFileSingle(string filename, string oldValue, string newValue)
	{
		string text = File.ReadAllText(filename);
		text = text.Replace(oldValue, newValue);
		File.WriteAllText(filename, text);
	}

	static void PatchFile(string filename, Dictionary<string, string> Replacements)
	{
		if (Replacements.Count == 0)
			return;

		string text = File.ReadAllText(filename);
		foreach (var KeyValue in Replacements)
		{
			text = text.Replace(KeyValue.Key, KeyValue.Value);
		}
		File.WriteAllText(filename, text);
	}

	static string IntToString(int value)
	{
		return value.ToString();
	}

	static string Int64ToString(long value)
	{
		return value.ToString();
	}

	static string BoolToIntString(bool value)
	{
		return value ? "1" : "0";
	}

	static void PrintBlue(string String)
	{
		Console.ForegroundColor = ConsoleColor.Blue;
		Console.WriteLine(String);
		Console.ResetColor();
	}

	static void PrintRed(string String)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(String);
		Console.ResetColor();
	}

	static void PrintGreen(string String)
	{
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine(String);
		Console.ResetColor();
	}

	static void PrintYellow(string String)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine(String);
		Console.ResetColor();
	}

	//
	// Print out a build message
	// Why error? Well, the UE masks all other errors. *shrug*
	//
	private void Trace(string msg)
	{
		Log.TraceInformation("Uwajimaya: " + msg);
	}

	// Trace helper
	private void Trace(string format, params object[] args)
	{
		Trace(string.Format(format, args));
	}

	// Raise an error
	private void Fail(string message)
	{
		Trace(message);
		throw new Exception(message);
	}

	public static bool IsLinux
	{
		get
		{
		   return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		}
	}

	public static bool ExistsOnPath(string exeName)
	{
		try
		{
			using (Process p = new Process())
			{
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = IsLinux ? "whereis" : "where";
				p.StartInfo.Arguments = exeName;
				p.Start();
				p.WaitForExit();
				return p.ExitCode == 0;
			}
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool RunCommand(string workingDirectory,  string commandName, string arguments, ref string output)
	{
		using (Process p = new Process())
		{
			p.StartInfo.WorkingDirectory = workingDirectory;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = commandName;
			p.StartInfo.Arguments = arguments;
			p.Start();
			output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			return p.ExitCode == 0;
		}
	}

	public static bool GetGitBranch(string path, ref string branch)
	{
		if (RunCommand(path, "git", "rev-parse --abbrev-ref HEAD", ref branch))
		{
			branch = branch.Trim();
			return true;
		}
		return false;
	}

	public static bool GetGitCommit(string path, ref string commit)
	{
		if (RunCommand(path, "git", "rev-parse --short HEAD", ref commit))
		{
			commit = commit.Trim();
			return true;
		}
		return false;
	}

	public bool GetCurrentBranchAndCommit(ref string branch, ref string commit)
	{
		// Try first from git command
		if (GeBranchAndCommitFromGit(ref branch, ref commit))
			return true;

		return false;
	}

	public bool GeBranchAndCommitFromGit(ref string branch, ref string commit)
	{
		if (!ExistsOnPath("git"))
		{
			PrintYellow("git does not exist in path");
			return false;
		}

		PrintBlue("Using git");
		if (!GetGitBranch(ProjectRootPath, ref branch))
		{
			PrintRed("Can't get git branch");
			return false;
		}
		if (!GetGitCommit(ProjectRootPath, ref commit))
		{
			PrintRed("Can't get git commit");
			return false;
		}
		return true;
	}

	//
	// END COMMON
	//
}
