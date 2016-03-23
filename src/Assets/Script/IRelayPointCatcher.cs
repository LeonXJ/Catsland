
namespace Catslandx {
  public interface IRelayPointCatcher {
    bool isSupportRelay();
    bool setRelayPoint(RelayPoint relayPoint);
    bool cancelRelayPoint(RelayPoint relayPoint);
  }
}
