using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EGS
{
/// <summary>
/// The names of the events in the game. Events MUST be added in alphabetical
/// order.
/// </summary>
public enum EventName
{
	DeathEvent,
	GameOverEvent,
	HealthUpdatedEvent,
	LivesUpdatedEvent,
	SpacePressedEvent // Event used for EventManager example
}
}

