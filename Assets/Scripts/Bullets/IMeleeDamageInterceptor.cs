namespace Catsland.Scripts.Bullets {
  public enum MeleeResultStatus {
    // No any effect.
    VOID = 0,

    // Hit the object.
    HIT = 1,

    // Not hit the object and apply repel on the subject.
    COUNTER_REPEL = 2,
  }

  public class MeleeResult {

    public static readonly MeleeResult VOID = new MeleeResult(MeleeResultStatus.VOID);
    public static readonly MeleeResult HIT = new MeleeResult(MeleeResultStatus.HIT);

    public MeleeResult(MeleeResultStatus status) {
      this.status = status;
    }

    public MeleeResultStatus status;
  }

  public interface IMeleeDamageInterceptor {
    MeleeResult getMeleeResult();
  }
}
