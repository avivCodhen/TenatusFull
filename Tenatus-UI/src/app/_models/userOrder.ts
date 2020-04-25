export interface UserOrder {
  externalId: string;
  created: string;
  quantity: number;
  price: number;
  isBuy: boolean;
  orderAction: string;
  stock: string;
}
