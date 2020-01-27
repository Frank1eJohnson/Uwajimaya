// Copyright (c) Csaba Molnar & Daniel Butum. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "UI/InGame/SoUIGameActivity.h"

#include "SoUIDialogueWithWizard.generated.h"


class UImage;
class UFMODEvent;
class UTextBlock;
class USoUICommandTooltip;
class UTexture;
class UWidgetAnimation;
class UDlgContext;

namespace FMOD
{
	namespace  Studio
	{
		class EventInstance;
	}
}

/**  UI and control class for mini dialogues with the wizard */
UCLASS()
class SORB_API USoUIDialogueWithWizard : public USoInGameUIActivity
{
	GENERATED_BODY()

protected:
	// Begin UUserWidget Interface
	void NativeConstruct() override;
	void NativeDestruct() override;
	// End UUserWidget Interface

	/** valid, started DlgContext is expected */
	bool SetInGameActivityEnabled_Implementation(UObject* Source, bool bEnable) override;

	bool HandleCommand_Implementation(ESoUICommand Command) override;

	bool Update_Implementation(float DeltaSeconds) override;

	void UpdatePlayerImage();

	void UpdateTooltipsVisbility();

	void CheckAndPlayVoice();

	UTexture* GetUwajimayaImage() const;

protected:

	UFUNCTION(BlueprintImplementableEvent)
	void FixTextSizes();

protected:

	UPROPERTY(BlueprintReadWrite, meta = (BindWidget), Category = Widgets)
	UImage* Background;

	UPROPERTY(BlueprintReadWrite, meta = (BindWidget), Category = Widgets)
	UTextBlock* WizardText;

	UPROPERTY(BlueprintReadWrite, meta = (BindWidget), Category = Widgets)
	UTextBlock* UwajimayaText;

	UPROPERTY(BlueprintReadWrite, meta = (BindWidget), Category = Widgets)
	USoUICommandTooltip* CommandTooltip;

	UPROPERTY(EditAnywhere, Category = Widgets)
	UTexture* UwajimayaImageClassic;

	UPROPERTY(EditAnywhere, Category = Widgets)
	UTexture* UwajimayaImageMummy;

	UPROPERTY(EditAnywhere, Category = Widgets)
	UTexture* UwajimayaImagePumpkin;


	UPROPERTY(BlueprintReadWrite, Category = Widgets)
	UWidgetAnimation* WidgetAnimFadeIn;

	UPROPERTY(BlueprintReadWrite, Category = Widgets)
	UWidgetAnimation* WidgetAnimFadeInWizardText;

	UPROPERTY(BlueprintReadWrite, Category = Widgets)
	UWidgetAnimation* WidgetAnimFadeInUwajimayaText;

	UPROPERTY(BlueprintReadWrite, Category = Widgets)
	UWidgetAnimation* WidgetAnimFadeOut;

	UPROPERTY(BlueprintReadWrite, Category = Widgets)
	UWidgetAnimation* WidgetAnimFadeOutWizardText;

	UPROPERTY(BlueprintReadWrite, Category = Widgets)
	UWidgetAnimation* WidgetAnimFadeOutUwajimayaText;

	UPROPERTY(BlueprintReadWrite, Category = Widgets)
	UWidgetAnimation* WidgetAnimSwitchBackground;


	bool bLastLineWasWizard = false;

	UPROPERTY(BlueprintReadOnly)
	UDlgContext* ActiveDlgContext = nullptr;

	bool bOpened = false;

	UPROPERTY(EditAnywhere)
	float SlowDownSpeed = 500.0f;

	bool bWaitUntilAnimEndWithVO;


	FMOD::Studio::EventInstance* Instance = nullptr;

	//
	// SFX
	//

	UPROPERTY(EditAnywhere, Category = SFX)
	UFMODEvent* SFXOnOpen = nullptr;

	UPROPERTY(EditAnywhere, Category = SFX)
	UFMODEvent* SFXOnClose = nullptr;

	UPROPERTY(EditAnywhere, Category = SFX)
	UFMODEvent* SFXOnSpeakerChange = nullptr;

	UPROPERTY(EditAnywhere, Category = SFX)
	UFMODEvent* SFXOnTextSwitch = nullptr;
};
