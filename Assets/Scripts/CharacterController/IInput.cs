namespace Catsland.Scripts.CharacterController {
  interface IInput {

    float getHorizontal();

    float getVertical();

    bool jump();

    bool attack();
  }
}
