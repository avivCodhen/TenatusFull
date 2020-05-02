import { Strategy } from './strategy';
import { UserOrder } from './userOrder';
import { StockData } from './stockData';
export interface Dashboard {
  userOrders: UserOrder[];
  isTraderOn: boolean;
  strategies: Strategy[];
  stocks: StockData[];
  marketOpen: boolean;
}
