import type { components } from "@/libs/api.schema.g";
import { FormField } from "@/components/FormField";
import { Input } from "@/components/ui/input";

type UpdateSalaryDto = components["schemas"]["UpdateJobApplicationSalaryDto"];

const CURRENCY_OPTIONS = ["USD", "EUR", "PLN", "GBP", "CHF", "CAD", "AUD", "SEK", "NOK", "DKK"];

interface JobApplicationSalaryFieldsProps {
  salary: UpdateSalaryDto;
  salaryLabel: string;
  onAmountChange: (
    salaryType: number | string,
    field: "offerFrom" | "offerTo" | "expectedFrom" | "expectedTo",
    value: number | null,
  ) => void;
  onCurrencyChange: (salaryType: number | string, currency: string) => void;
}

const parseSalaryAmount = (rawValue: string): number | null => {
  if (!rawValue.trim()) {
    return null;
  }

  const parsed = Number(rawValue);
  if (Number.isNaN(parsed)) {
    return null;
  }

  return parsed;
};

export function JobApplicationSalaryFields({
  salary,
  salaryLabel,
  onAmountChange,
  onCurrencyChange,
}: JobApplicationSalaryFieldsProps) {
  return (
    <div className="rounded-md border p-3">
      <div className="mb-2 text-sm font-medium">{salaryLabel}</div>
      <div className="grid grid-cols-1 gap-3 md:grid-cols-5">
        <FormField label="Offer from">
          <Input
            type="number"
            value={salary.offerFrom?.toString() ?? ""}
            onChange={(event) => onAmountChange(salary.salaryType, "offerFrom", parseSalaryAmount(event.target.value))}
          />
        </FormField>
        <FormField label="Offer to">
          <Input
            type="number"
            value={salary.offerTo?.toString() ?? ""}
            onChange={(event) => onAmountChange(salary.salaryType, "offerTo", parseSalaryAmount(event.target.value))}
          />
        </FormField>
        <FormField label="Expected from">
          <Input
            type="number"
            value={salary.expectedFrom?.toString() ?? ""}
            onChange={(event) => onAmountChange(salary.salaryType, "expectedFrom", parseSalaryAmount(event.target.value))}
          />
        </FormField>
        <FormField label="Expected to">
          <Input
            type="number"
            value={salary.expectedTo?.toString() ?? ""}
            onChange={(event) => onAmountChange(salary.salaryType, "expectedTo", parseSalaryAmount(event.target.value))}
          />
        </FormField>
        <FormField label="Currency">
          <select
            className="h-9 w-full rounded-md border bg-background px-3 text-sm"
            value={salary.currency ?? ""}
            onChange={(event) => onCurrencyChange(salary.salaryType, event.target.value)}
          >
            <option value=""></option>
            {salary.currency && !CURRENCY_OPTIONS.includes(salary.currency) ? (
              <option value={salary.currency}>{salary.currency}</option>
            ) : null}
            {CURRENCY_OPTIONS.map((currency) => (
              <option key={currency} value={currency}>
                {currency}
              </option>
            ))}
          </select>
        </FormField>
      </div>
    </div>
  );
}
