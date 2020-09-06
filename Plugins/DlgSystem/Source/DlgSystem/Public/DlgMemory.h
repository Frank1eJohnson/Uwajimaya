// Copyright Csaba Molnar, Daniel Butum. All Rights Reserved.
#pragma once

#include "CoreMinimal.h"

#include "DlgMemory.generated.h"

USTRUCT(BlueprintType)
struct DLGSYSTEM_API FDlgHistory
{
	GENERATED_USTRUCT_BODY()
public:
	FDlgHistory() {}

	void Add(int32 NodeIndex, const FGuid& NodeGUID)
	{
		if (NodeIndex >= 0)
		{
			VisitedNodeIndices.Add(NodeIndex);
		}
		if (NodeGUID.IsValid())
		{
			VisitedNodeGUIDs.Add(NodeGUID);
		}
	}

	// The following scenarios will be present:
	//
	// ---------------------------------------------------------------------------------------
	// A. Scenario
	// - User has save files with only VisitedNodeIndices present
	// - Developer does NOT resave the Dialogue files so that the Nodes don't have a valid GUID
	//
	// Result:
	//     The history will just use the Node Indices because the GUID will not be valid for any Node
	//
	// ---------------------------------------------------------------------------------------
	// B. Scenario
	// - User has save files with only the VisitedNodeIndices present
	// - Developer DOES resave the Dialogue files so that the Nodes have a valid GUID
	//
	// Result:
	//     The history will still use the Node Indices to figure out if a Node Is Visited,
	//     even tho the Dialogue Nodes have a valid GUIDs we won't use those because the user save files
	//     has VisitedNodeGUIDs.Num() < VisitedNodeIndices.Num() (the user most likely has empty VisitedNodeGUIDs),
	//     because the way the history is constructed now the following is a requirement
	//     VisitedNodeGUIDs.Num() >= VisitedNodeIndices.Num(), if this is not met we just fallback to the Node Indices
	//
	// ---------------------------------------------------------------------------------------
	// C. Scenario
	// - User has save files with the VisitedNodeIndices present and some VisitedNodeGUIDs but not all of them,
	//   because he only got the new update after he played some time through the game
	// - Developer DOES resave the Dialogue files so that the Nodes have a valid GUID and the game is in the users
	//   hands for some time for them to play.
	//
	// Result:
	//     Same as in Scenario B we will still use the Node Indices, because the requirement
	//     VisitedNodeGUIDs.Num() >= VisitedNodeIndices.Num() is NOT met
	bool CanUseGUIDForSearch() const
	{
		return VisitedNodeGUIDs.Num() >= VisitedNodeIndices.Num();
	}

	bool Contains(int32 NodeIndex, const FGuid& NodeGUID) const
	{
		// Use GUID
		if (CanUseGUIDForSearch() && NodeGUID.IsValid())
		{
			return VisitedNodeGUIDs.Contains(NodeGUID);
		}

		// FallBack to Node Index
		return VisitedNodeIndices.Contains(NodeIndex);
	}

	bool operator==(const FDlgHistory& Other) const;

public:
	// Sed of already visited Node indices
	// NOTE: if you serialize this but then later change the dialogue node positions this will have the wrong indices
	// NOTE: You should use VisitedNodeGUIDs
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Dialogue|History")
	TSet<int32> VisitedNodeIndices;

	// Set of already visited node GUIDs
	// This was added to fix Issue 30 (https://gitlab.com/Sabrave/DlgSystem/-/issues/30)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Dialogue|History")
	TSet<FGuid> VisitedNodeGUIDs;
};

// Singleton to store Dialogue history
// TODO: investigate if this is multiplayer friendly, it does not seem so as there exists only a single global dialogue memory
USTRUCT()
struct DLGSYSTEM_API FDlgMemory
{
	GENERATED_USTRUCT_BODY()
public:
	FDlgMemory() {}
	static FDlgMemory* GetInstance()
	{
		static FDlgMemory Instance;
		return &Instance;
	}
	static FDlgMemory& Get()
	{
		auto* Instance = GetInstance();
		check(Instance != nullptr);
		return *Instance;
	}

	// Removes all entries
	void Empty() { HistoryMap.Empty(); }

	// Adds an entry to the map or overrides an existing one
	void SetEntry(const FGuid& DialogueGUID, const FDlgHistory& History)
	{
		FDlgHistory* OldEntry = HistoryMap.Find(DialogueGUID);

		if (OldEntry == nullptr)
		{
			HistoryMap.Add(DialogueGUID, History);
		}
		else
		{
			*OldEntry = History;
		}
	}

	// Returns the entry for the given name, or nullptr if it does not exist */
	FDlgHistory* GetEntry(const FGuid& DialogueGUID) { return HistoryMap.Find(DialogueGUID); }

	void SetNodeVisited(const FGuid& DialogueGUID, int32 NodeIndex, const FGuid& NodeGUID)
	{
		// Add it if it does not exist already
		FDlgHistory* History = HistoryMap.Find(DialogueGUID);
		if (History == nullptr)
		{
			History = &HistoryMap.Add(DialogueGUID);
		}

		History->Add(NodeIndex, NodeGUID);
	}

	bool IsNodeVisited(const FGuid& DialogueGUID, int32 NodeIndex, const FGuid& NodeGUID) const
	{
		// Dialogue entry does not even exist
		const FDlgHistory* History = HistoryMap.Find(DialogueGUID);
		if (History == nullptr)
		{
			return false;
		}

		return History->Contains(NodeIndex, NodeGUID);
	}

	bool IsNodeIndexVisited(const FGuid& DialogueGUID, int32 NodeIndex) const
	{
		// Dialogue entry does not even exist
		const FDlgHistory* History = HistoryMap.Find(DialogueGUID);
		if (History == nullptr)
		{
			return false;
		}

		return History->VisitedNodeIndices.Contains(NodeIndex);
	}

	bool IsNodeGUIDVisited(const FGuid& DialogueGUID, const FGuid& NodeGUID) const
	{
		// Dialogue entry does not even exist
		const FDlgHistory* History = HistoryMap.Find(DialogueGUID);
		if (History == nullptr)
		{
			return false;
		}

		return History->VisitedNodeGUIDs.Contains(NodeGUID);
	}

	const TMap<FGuid, FDlgHistory>& GetHistoryMaps() const { return HistoryMap; }
	void SetHistoryMap(const TMap<FGuid, FDlgHistory>& Map) { HistoryMap = Map; }

private:
	 // Key: Dialogue unique identifier GUID
	 // Value: set of already visited nodes
	UPROPERTY()
	TMap<FGuid, FDlgHistory> HistoryMap;
};

template<>
struct TStructOpsTypeTraits<FDlgHistory> : public TStructOpsTypeTraitsBase2<FDlgHistory>
{
	enum
	{
		WithIdenticalViaEquality = true
	};
};
