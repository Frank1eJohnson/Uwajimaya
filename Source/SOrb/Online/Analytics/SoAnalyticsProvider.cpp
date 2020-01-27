// Copyright (c) Csaba Molnar & Daniel Butum. All Rights Reserved.
#include "SoAnalyticsProvider.h"

#if UWAJIMAYA_WITH_GAMEANALYTICS
#include "GameAnalytics.h"
#include "UEGameAnalytics.h"
#endif // UWAJIMAYA_WITH_GAMEANALYTICS

#if UWAJIMAYA_WITH_ANALYTICS
#include "Analytics.h"
#endif // UWAJIMAYA_WITH_ANALYTICS

#include "Basic/Helpers/SoPlatformHelper.h"

DEFINE_LOG_CATEGORY_STATIC(LogSoAnalyticsProvider,  All, All);

#if UWAJIMAYA_WITH_ANALYTICS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
TSharedPtr<IAnalyticsProvider> FSoAnalyticsProvider::GetDefaultConfiguredProvider(const FString& ContextErrorMessage)
{
#if UWAJIMAYA_WITH_GAMEANALYTICS
	static TSharedPtr<IAnalyticsProvider> Provider = FAnalytics::Get().GetDefaultConfiguredProvider();
	if (!Provider.IsValid() && !ContextErrorMessage.IsEmpty())
	{
		const FString Message = FString::Printf(
			TEXT("%s: Failed to get the default analytics provider. Double check your [Analytics] configuration in your INI"), *ContextErrorMessage);
		UE_LOG(LogSoAnalyticsProvider, Error, TEXT("%s"), *Message);
	}
	return Provider;
#else
	return nullptr;
#endif // UWAJIMAYA_WITH_GAMEANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordErrorWithAttributes(const FString& Error, const TArray<FAnalyticsEventAttribute>& Attributes)
{
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordErrorWithAttributes"));
	if (Provider.IsValid())
	{
		Provider->RecordError(Error, Attributes);
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordProgressWithFullHierarchyAndAttributes(const FString& ProgressType, const TArray<FString>& ProgressNames, const TArray<FAnalyticsEventAttribute>& Attributes)
{
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordProgressWithFullHierarchyAndAttributes"));
	if (Provider.IsValid())
	{
		Provider->RecordProgress(ProgressType, ProgressNames, Attributes);
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordProgressWithAttributes(const FString& ProgressType, const FString& ProgressName, const TArray<FAnalyticsEventAttribute>& Attributes)
{
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordProgressWithAttributes"));
	if (Provider.IsValid())
	{
		Provider->RecordProgress(ProgressType, ProgressName, Attributes);
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
bool FSoAnalyticsProvider::StartSessionWithAttributes(const TArray<FAnalyticsEventAttribute>& Attributes)
{
	InitializeBuildOverride();

	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("StartSessionWithAttributes"));
	if (Provider.IsValid())
	{
		const bool bStatus = Provider->StartSession(Attributes);
		return bStatus;
	}

	return false;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordEventWithAttributes(const FString& EventName, const TArray<FAnalyticsEventAttribute>& Attributes)
{
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordEventWithAttributes"));
	if (Provider.IsValid())
	{
		Provider->RecordEvent(EventName, Attributes);
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordItemPurchaseWithAttributes(const FString& ItemId, int32 ItemQuantity, const TArray<FAnalyticsEventAttribute>& Attributes)
{
	// The only valid RecordItemPurchase supported by GameAnalytics
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordSimpleItemPurchaseWithAttributes"));
	if (Provider.IsValid())
	{
		Provider->RecordItemPurchase(ItemId, ItemQuantity, Attributes);
	}
}
#endif // UWAJIMAYA_WITH_ANALYTICS


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::InitializeBuildOverride()
{
#if UWAJIMAYA_WITH_GAMEANALYTICS
	FGameAnalyticsBuildOverride& BuildOverride = FAnalyticsGameAnalytics::Get().GetBuildOverride();
	if (!BuildOverride.IsBound())
	{
		BuildOverride.BindLambda(&USoPlatformHelper::GetGameBuildVersion);
	}
#endif // UWAJIMAYA_WITH_GAMEANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
bool FSoAnalyticsProvider::StartSession()
{
	InitializeBuildOverride();

#if UWAJIMAYA_WITH_ANALYTICS
	// Start the normal sessions
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("StartSession"));
	if (Provider.IsValid())
	{
		const bool bStatus = Provider->StartSession();
		// Be aware that the initialization will always automatically start the first session even with manual session handling.
		// NOTE: On PIE  Mode because we have manual session handling enabled it will just call UGameAnalytics::startSession()
		return bStatus;
	}
#endif // UWAJIMAYA_WITH_ANALYTICS

	return false;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::EndSession()
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("EndSession"));
	if (Provider.IsValid())
	{
		Provider->EndSession();
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::FlushEvents()
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("FlushEvents"));
	if (Provider.IsValid())
	{
		Provider->FlushEvents();
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::SetCustomDimensions()
{
#if UWAJIMAYA_WITH_GAMEANALYTICS

#if WITH_EDITOR
	UGameAnalytics::setCustomDimension01("WITH_EDITOR");
#else
	UGameAnalytics::setCustomDimension01("WITH_NO_EDITOR");
#endif // WITH_EDITOR

#endif // UWAJIMAYA_WITH_GAMEANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::WaitForAnalyticsToSend()
{
#if UWAJIMAYA_WITH_GAMEANALYTICS
	UGameAnalytics::waitUntilJobsAreDone();
#endif // UWAJIMAYA_WITH_GAMEANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::SetPollTimers(double WaitSeconds, double ProcessEventsSeconds)
{
#if UWAJIMAYA_WITH_GAMEANALYTICS
	UGameAnalytics::setThreadAndEventTimers(WaitSeconds, ProcessEventsSeconds);
#endif // UWAJIMAYA_WITH_GAMEANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordEvent(const FString& EventName)
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordEvent"));
	if (Provider.IsValid())
	{
		Provider->RecordEvent(EventName);
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordEventWithAttribute(const FString& EventName, const FString& AttributeName, const FString& AttributeValue)
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordEventWithAttribute"));
	if (Provider.IsValid())
	{
		const FAnalyticsEventAttribute Attribute{AttributeName, AttributeValue};
		Provider->RecordEvent(EventName, Attribute);
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
FString FSoAnalyticsProvider::GetSessionId()
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("GetSessionId"));
	if (Provider.IsValid())
	{
		return Provider->GetSessionID();
	}
#endif // UWAJIMAYA_WITH_ANALYTICS

	return FString();
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::SetSessionId(const FString& SessionId)
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("SetSessionId"));
	if (Provider.IsValid())
	{
		Provider->SetSessionID(SessionId);
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
FString FSoAnalyticsProvider::GetUserId()
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("GetUserId"));
	if (Provider.IsValid())
	{
		return Provider->GetUserID();
	}
#endif // UWAJIMAYA_WITH_ANALYTICS

	return FString();
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::SetUserId(const FString& UserId)
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("SetUserId"));
	if (Provider.IsValid())
	{
		Provider->SetUserID(UserId);
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::SetBuildInfo(const FString& BuildInfo)
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("SetBuildInfo"));
	if (Provider.IsValid())
	{
		Provider->SetBuildInfo(BuildInfo);
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void FSoAnalyticsProvider::RecordProgress(const FString& ProgressType, const FString& ProgressName)
{
#if UWAJIMAYA_WITH_ANALYTICS
	TSharedPtr<IAnalyticsProvider> Provider = GetDefaultConfiguredProvider(TEXT("RecordProgress"));
	if (Provider.IsValid())
	{
		Provider->RecordProgress(ProgressType, ProgressName);
	}
#endif // UWAJIMAYA_WITH_ANALYTICS
}
