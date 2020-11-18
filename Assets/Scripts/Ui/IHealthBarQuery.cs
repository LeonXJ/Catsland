using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;

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

    public static HealthCondition CreateHealthCondition(VulnerableAttribute vulnerableAttribute, string name = "") {
      return new HealthCondition(vulnerableAttribute.maxHealth, vulnerableAttribute.currentHealth, name);
    }
  }

  public interface IHealthBarQuery {
    HealthCondition GetHealthCondition();

  }
}
