namespace Catsland.Scripts.Controller {
  interface IInput {

    float getHorizontal();

    float getVertical();

    bool jump();

    bool attack();

    bool dash();

    bool meditation();
  }
}
