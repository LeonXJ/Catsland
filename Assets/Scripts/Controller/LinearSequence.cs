using UnityEngine;
using System.Collections.Generic;
using System;

public class LinearSequence {

  public delegate bool IsActionDone();

  public class Action {
    public const long INFINIT_TIME = 99999999L;

    public Enum actionStatus;
    public float performingTime;
    public IsActionDone isActionDoneFunction;

    public Action(Enum actionStatus, float performingTime, IsActionDone isActionDoneFunction = null) {
      this.actionStatus = actionStatus;
      this.performingTime = performingTime;
      this.isActionDoneFunction = isActionDoneFunction;
    }
  }

  private List<Action> actions;
  private Enum jumpToStatusWhenFinish;
  private Dictionary<Enum, int> statusToOrder;
  private float currentStatusRemainingTime;


  private LinearSequence(List<Action> actions, Enum endingStatus) {
    this.actions = actions;
    this.jumpToStatusWhenFinish = endingStatus;

    statusToOrder = new Dictionary<Enum, int>();
    for(int i = 0; i < actions.Count; ++i) {
      statusToOrder.Add(actions[i].actionStatus, i);
    }
  }

  public Enum start() {
    currentStatusRemainingTime = actions[0].performingTime;
    return actions[0].actionStatus;
  }

  public Enum processIfInInterestedStatus(Enum actionStatus) {
    if(!statusToOrder.ContainsKey(actionStatus)) {
      return actionStatus;
    }
    float timeToProcess = Time.deltaTime;
    Enum currentStatus = actionStatus;
    int statusOrder = statusToOrder[actionStatus];
    while(timeToProcess > 0.0f) {
      Action currentAction = actions[statusOrder];
      // keep in current status if:
      // 1) no skip function or skip functions says no
      // 2) time is not used up
      if((currentAction.isActionDoneFunction == null
          || !actions[statusOrder].isActionDoneFunction())
        && currentStatusRemainingTime > timeToProcess) {
        currentStatusRemainingTime -= timeToProcess;
        return currentStatus;
      }
      // otherwise, jump to next status
      if(currentAction.isActionDoneFunction == null) {
        timeToProcess -= currentStatusRemainingTime;
      }
      if(statusOrder == actions.Count - 1) {
        return jumpToStatusWhenFinish;
      }
      statusOrder += 1;
      currentStatus = actions[statusOrder].actionStatus;
      currentStatusRemainingTime = actions[statusOrder].performingTime;
    }
    return currentStatus;
  }

  public static LinearSequenceBuilder newBuilder() {
    return new LinearSequenceBuilder();
  }


  public class LinearSequenceBuilder {
    private List<Action> actions;
    private Enum endingStatus;
    private bool isEndingStatusDefined = false;

    public LinearSequenceBuilder() {
      actions = new List<Action>();
    }

    public LinearSequenceBuilder append(Enum actionStatus, float performingTime) {
      actions.Add(new Action(actionStatus, performingTime));
      return this;
    }

    public LinearSequenceBuilder append(Enum actionStatus, IsActionDone isActionDone) {
      actions.Add(new Action(actionStatus, Action.INFINIT_TIME, isActionDone));
      return this;
    }

    public LinearSequenceBuilder withEndingStatus(Enum actionStatus) {
      endingStatus = actionStatus;
      isEndingStatusDefined = true;
      return this;
    }

    public LinearSequence build() {
      Debug.Assert(actions.Count > 1, "Cannot build LinearSequence with less than 2 actions.");
      Debug.Assert(isEndingStatusDefined, "Ending status must be set.");
      return new LinearSequence(actions, endingStatus);
    }
  }
}


