namespace Catsland.Scripts.Controller {
  interface IInput {

    float getHorizontal();

    float getVertical();

    bool jump();

    bool jumpHigher();

    bool attack();

    bool dash();

    bool meditation();

    bool interact();
  }
}
