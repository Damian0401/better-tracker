type Option = {
  value: number | string;
  name: string;
};

export const getOptionLabel = (options: Option[], value: number | string): string => {
  const match = options.find((option) => option.value.toString() === value.toString());
  return match?.name ?? "";
};
