export const formatDate = (date: string): string => {
  const value = new Date(date);
  if (Number.isNaN(value.getTime())) {
    return "";
  }

  return value.toLocaleDateString();
};
