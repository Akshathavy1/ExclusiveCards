CREATE TABLE [Exclusive].[PlanAgentHistory]
(
    [Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	[MembershipPlanId] INT NOT NULL,
	[AgentCodeId] INT NULL,
	[Assigned] DATETIME2 (7) DEFAULT GETDATE() NOT NULL
)
