import { Strategy } from './strategy';
import { UserOrder } from './userOrder';
export interface Dashboard {
  userOrders: UserOrder[];
  isOn: boolean;
  strategies: Strategy[];
}
