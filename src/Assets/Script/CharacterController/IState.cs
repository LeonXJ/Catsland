using System.Collections.Generic;

namespace Catslandx {
  public interface IState {

    void onEnter(IState previousState);

    IState update(Dictionary<SensorEnum, ISensor> sensors, ICharacterInput input, float deltaTime);

    void onExit(IState nextState);
  }
}
