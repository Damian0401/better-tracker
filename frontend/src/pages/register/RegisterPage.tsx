import { useState } from "react";
import { Link, useNavigate } from "@tanstack/react-router";
import { Api } from "@/libs/api";
import { Auth } from "@/libs/auth";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { toast } from "sonner";
import { Routes } from "@/constants";

export function RegisterPage() {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    userName: "",
    login: "",
    password: "",
    confirmPassword: "",
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    if (formData.password !== formData.confirmPassword) {
      toast.error("Passwords do not match");
      setIsLoading(false);
      return;
    }

    try {
      const response = await Api.POST("/api/v1/auth/register", {
        body: {
          userName: formData.userName,
          login: formData.login,
          password: formData.password,
        },
      });

      if (response.data) {
        Auth.setToken(response.data.token);
        Auth.setUser({
          userId: response.data.userId,
          userName: response.data.userName,
        });
        navigate({ to: Routes.HOME });
      } else {
        const error = response.error as unknown as { errors: string[] } | undefined;
        const errors = error?.errors || [];
        toast.error(errors.join(", ") || "Registration failed");
      }
    } catch {
      toast.error("An unexpected error occurred");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 p-4">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Register</CardTitle>
          <CardDescription>
            Create a new account to get started
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Input
                type="text"
                placeholder="Username"
                value={formData.userName}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, userName: e.target.value }))
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Input
                type="text"
                placeholder="Login"
                value={formData.login}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, login: e.target.value }))
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Input
                type="password"
                placeholder="Password"
                value={formData.password}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, password: e.target.value }))
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Input
                type="password"
                placeholder="Confirm Password"
                value={formData.confirmPassword}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, confirmPassword: e.target.value }))
                }
                required
              />
            </div>
            <Button type="submit" className="w-full" disabled={isLoading}>
              {isLoading ? "Registering..." : "Register"}
            </Button>
          </form>
          <p className="mt-4 text-center text-sm text-muted-foreground">
            Already have an account?{" "}
            <Link to={Routes.LOGIN} className="text-primary hover:underline">
              Login
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}