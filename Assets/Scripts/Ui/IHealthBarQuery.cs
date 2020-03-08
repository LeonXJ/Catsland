using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Ui {

  public class HealthCondition {
    public int totalHealth;
    public int currentHealth;
    public string name;

    public HealthCondition(int totalHealth, int currentHealth, string name = "") {
      this.totalHealth = totalHealth;
      this.currentHealth = currentHealth;
      this.name = name;
    }
  }
  public interface IHealthBarQuery {
    HealthCondition GetHealthCondition();

  }
}
