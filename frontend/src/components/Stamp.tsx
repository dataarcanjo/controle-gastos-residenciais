import "./Stamp.css";

interface StampProps {
  positive: boolean;
}

/**
 * Elemento de assinatura visual do app: um "carimbo" de tinta indicando se o
 * saldo de uma pessoa está positivo ou negativo, remetendo a carimbos usados
 * em livros-caixa e extratos bancários físicos.
 */
export function Stamp({ positive }: StampProps) {
  return (
    <span className={`stamp ${positive ? "stamp--brass" : "stamp--red"}`} aria-hidden="true">
      {positive ? "saldo ok" : "no vermelho"}
    </span>
  );
}
