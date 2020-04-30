export interface UserOrder {
  externalId: string;
  created: string;
  quantity: number;
  buyingPrice: number;
  isBuy: boolean;
  orderAction: string;
  stock: string;
  userOrderType: string;
}
