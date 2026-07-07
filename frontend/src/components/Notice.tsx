import "./Notice.css";

interface NoticeProps {
  kind: "error" | "success";
  message: string;
  onDismiss: () => void;
}

/**
 * Banner de notificação usado para exibir erros vindos da API (ex.: regra de
 * negócio violada) ou confirmações de sucesso, em linguagem direta e sem culpar
 * a pessoa usuária.
 */
export function Notice({ kind, message, onDismiss }: NoticeProps) {
  return (
    <div className={`notice notice--${kind}`} role="status">
      <span>{message}</span>
      <button type="button" className="notice__dismiss" onClick={onDismiss} aria-label="Fechar aviso">
        ×
      </button>
    </div>
  );
}
