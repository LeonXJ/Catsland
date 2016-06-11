using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface ICharacterController2D {
    void move(float horitonzalSpeed, bool jump, bool dash, bool croush);
    void getHurt(int hurtPoint);
    void die();
    void reset();
  }
}
