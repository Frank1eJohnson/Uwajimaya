// Copyright (c) Csaba Molnar & Daniel Butum. All Rights Reserved.

#pragma once

#include "Logging/LogMacros.h"
#include "Modules/ModuleInterface.h"
#include "Modules/ModuleManager.h"

DECLARE_LOG_CATEGORY_EXTERN(LogSoGeneral, All, All);

const FName UWAJIMAYA_GAME_MODULE_NAME(TEXT("UwajimayaGame"));

class FUwajimayaGameModule: public IModuleInterface
{
	typedef FUwajimayaGameModule Self;
public:
	void StartupModule() override;
	void ShutdownModule() override;
	bool IsGameModule() const override
	{
		return true;
	}

	static FUwajimayaGameModule& Get() { return FModuleManager::LoadModuleChecked<FUwajimayaGameModule>(UWAJIMAYA_GAME_MODULE_NAME); }
	static bool IsAvailable() { return FModuleManager::Get().IsModuleLoaded(UWAJIMAYA_GAME_MODULE_NAME); }

	void Init();
	void InitStringTables();
	void InitInput();
	void InitPlugins();

	void HandleSwitchConfigSerialization();

protected:
	void HandlePreExit();
	void HandleExit();
	void HandleApplicationWillDeactivate();
	void HandleApplicationHasReactivated();
	void HandleApplicationWillEnterBackground();
	void HandleApplicationHasEnteredForeground();

protected:
	bool bInitStringTables = false;

	FDelegateHandle PreExitDelegateHandle;
	FDelegateHandle ExitDelegateHandle;

	FDelegateHandle ApplicationWillDeactivateDelegate;
	FDelegateHandle ApplicationHasReactivatedDelegate;
	FDelegateHandle ApplicationWillEnterBackgroundDelegate;
	FDelegateHandle ApplicationHasEnteredForegroundDelegate;
};
