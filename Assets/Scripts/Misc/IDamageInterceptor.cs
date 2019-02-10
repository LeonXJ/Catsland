using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Misc {
  public interface IDamageInterceptor {

    bool shouldFlashOnDamage(DamageInfo damageInfo);

    bool shouldSplashOnDamage(DamageInfo damageInfo);
  }
}
